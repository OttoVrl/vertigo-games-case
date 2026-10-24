using Runtime.Data;
using UnityEditor;
using UnityEngine;

namespace Editor.Data
{
    public class CollectibleDatabaseTracker : AssetPostprocessor
    {
        private static bool _isTracking;

        public static void Refresh(GameDatabaseConfig config)
        {
            if (config == null)
                return;

            var serializedConfig = new SerializedObject(config);
            serializedConfig.Update();
            SyncList<CurrencyCollectibleData>(serializedConfig.FindProperty("currencies"));
            SyncList<UnlockPointCollectibleData>(serializedConfig.FindProperty("unlockPoints"));
            SyncList<UpgradePointCollectibleData>(serializedConfig.FindProperty("upgradePoints"));
            serializedConfig.ApplyModifiedProperties();
        }

        public static void AddIfMissing(SerializedProperty list, Object asset)
        {
            if (list == null || asset == null || Contains(list, asset))
                return;

            int index = list.arraySize;
            list.InsertArrayElementAtIndex(index);
            list.GetArrayElementAtIndex(index).objectReferenceValue = asset;
        }

        public static void RemoveAt(SerializedProperty list, int index)
        {
            SerializedProperty element = list.GetArrayElementAtIndex(index);
            if (element.objectReferenceValue != null)
                element.objectReferenceValue = null;

            list.DeleteArrayElementAtIndex(index);
        }

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (_isTracking || importedAssets == null || importedAssets.Length == 0)
                return;

            _isTracking = true;
            try
            {
                TrackImportedCollectibles(importedAssets);
            }
            finally
            {
                _isTracking = false;
            }
        }

        private static void TrackImportedCollectibles(string[] importedAssets)
        {
            bool hasCollectible = false;
            for (int i = 0; i < importedAssets.Length; i++)
            {
                if (AssetDatabase.LoadAssetAtPath<BaseCollectibleData>(importedAssets[i]) != null)
                {
                    hasCollectible = true;
                    break;
                }
            }

            if (!hasCollectible)
                return;

            string[] configGuids = AssetDatabase.FindAssets("t:GameDatabaseConfig");
            for (int i = 0; i < configGuids.Length; i++)
            {
                string configPath = AssetDatabase.GUIDToAssetPath(configGuids[i]);
                var config = AssetDatabase.LoadAssetAtPath<GameDatabaseConfig>(configPath);
                if (config == null || !config.AutoTrackCollectibles)
                    continue;

                var serializedConfig = new SerializedObject(config);
                serializedConfig.Update();
                bool changed = false;

                for (int assetIndex = 0; assetIndex < importedAssets.Length; assetIndex++)
                {
                    var collectible = AssetDatabase.LoadAssetAtPath<BaseCollectibleData>(importedAssets[assetIndex]);
                    changed |= TryAdd(serializedConfig, collectible);
                }

                if (changed)
                    serializedConfig.ApplyModifiedProperties();
            }
        }

        private static bool TryAdd(SerializedObject serializedConfig, BaseCollectibleData collectible)
        {
            if (collectible == null)
                return false;

            string propertyName = GetListPropertyName(collectible);
            if (propertyName == null)
                return false;

            SerializedProperty list = serializedConfig.FindProperty(propertyName);
            if (list == null || Contains(list, collectible))
                return false;

            AddIfMissing(list, collectible);
            return true;
        }

        private static void SyncList<T>(SerializedProperty list) where T : BaseCollectibleData
        {
            if (list == null)
                return;

            RemoveNullsAndDuplicates(list);

            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                AddIfMissing(list, asset);
            }
        }

        private static void RemoveNullsAndDuplicates(SerializedProperty list)
        {
            for (int i = list.arraySize - 1; i >= 0; i--)
            {
                Object value = list.GetArrayElementAtIndex(i).objectReferenceValue;
                if (value == null || ContainsBefore(list, value, i))
                    RemoveAt(list, i);
            }
        }

        private static bool ContainsBefore(SerializedProperty list, Object asset, int index)
        {
            for (int i = 0; i < index; i++)
            {
                if (list.GetArrayElementAtIndex(i).objectReferenceValue == asset)
                    return true;
            }

            return false;
        }

        private static bool Contains(SerializedProperty list, Object asset)
        {
            for (int i = 0; i < list.arraySize; i++)
            {
                if (list.GetArrayElementAtIndex(i).objectReferenceValue == asset)
                    return true;
            }

            return false;
        }

        private static string GetListPropertyName(BaseCollectibleData collectible)
        {
            if (collectible is CurrencyCollectibleData)
                return "currencies";

            if (collectible is UnlockPointCollectibleData)
                return "unlockPoints";

            if (collectible is UpgradePointCollectibleData)
                return "upgradePoints";

            return null;
        }
    }
}
