using Editor.Utility;
using Runtime.Data;
using UnityEditor;
using UnityEngine;

namespace Editor.Data
{
    public class GameDatabaseWindow : EditorWindow
    {
        
        private const float RowHeight = 22f;
        private const float IconSize = 16f;
        private const float RemoveSize = 20f;
        private const float IdWidth = 110f;
        private const float ListWidthRatio = 0.3f;
        private const float DetailIconSize = 128f;

        private static readonly GroupDefinition[] Groups =
        {
            new GroupDefinition("Currency", "currencies", "Assets/Datas/CollectibleReferences/Currency", "CurrencyCollectibleData", typeof(CurrencyCollectibleData)),
            new GroupDefinition("Unlock Point", "unlockPoints", "Assets/Datas/CollectibleReferences/UnlockPoint", "UnlockPointCollectibleData", typeof(UnlockPointCollectibleData)),
            new GroupDefinition("Upgrade Point", "upgradePoints", "Assets/Datas/CollectibleReferences/UpgradePoint", "PointCollectibleData", typeof(UpgradePointCollectibleData)),
            new GroupDefinition("Chest Point", "chestPoints", "Assets/Datas/CollectibleReferences/ChestPoint", "ChestPointCollectibleData", typeof(ChestPointCollectibleData)),
            new GroupDefinition("Consumable", "consumables", "Assets/Datas/CollectibleReferences/ConsumableItem", "ConsumableCollectibleData", typeof(ConsumableCollectibleData))
        };

        [SerializeField] private GameDatabaseConfig _config;
        [SerializeField] private BaseCollectibleData _selected;

        private SerializedObject _serializedConfig;
        private SerializedObject _selectedSerialized;
        private Vector2 _listScroll;
        private Vector2 _detailScroll;

        [MenuItem("Tools/Vertigo/Database Window")]
        private static void OpenFromMenu()
        {
            var window = GetWindow<GameDatabaseWindow>("Database");
            GameDatabaseConfig config = Selection.activeObject as GameDatabaseConfig;
            if (config == null)
                config = window._config;

            if (config == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:GameDatabaseConfig");
                if (guids.Length > 0)
                    config = AssetDatabase.LoadAssetAtPath<GameDatabaseConfig>(AssetDatabase.GUIDToAssetPath(guids[0]));
            }

            if (config != null)
                Open(config);
            else
                window.Show();
        }

        public static void Open(GameDatabaseConfig config)
        {
            var window = GetWindow<GameDatabaseWindow>("Database");
            if (window._config != config)
            {
                window._config = config;
                window._selected = null;
                window._selectedSerialized = null;
            }

            window.EnsureSerializedObject();
            window.Show();
            window.Focus();
        }

        private void OnEnable()
        {
            minSize = new Vector2(720f, 360f);
            titleContent = new GUIContent("Database");
            EnsureSerializedObject();
            EnsureSelectedSerializedObject();
        }

        private void OnGUI()
        {
            EnsureSerializedObject();

            if (_config == null || _serializedConfig == null)
            {
                EditorGUILayout.HelpBox(
                    "DatabaseConfig seçili değil. Window/Database menüsünü veya asset inspector'ındaki Open Database Window butonunu kullan.",
                    MessageType.Info);
                return;
            }

            _serializedConfig.Update();
            DrawToolbar();

            EditorGUILayout.BeginHorizontal();
            DrawGroups();
            DrawDetail();
            EditorGUILayout.EndHorizontal();

            _serializedConfig.ApplyModifiedProperties();
        }

        private void DrawToolbar()
        {
            SerializedProperty autoTrack = _serializedConfig.FindProperty("autoTrackCollectibles");
            if (autoTrack == null)
                return;

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUI.BeginChangeCheck();
            bool track = EditorGUILayout.ToggleLeft(
                "Auto Track Project Collectibles",
                autoTrack.boolValue,
                GUILayout.Width(240f));
            if (EditorGUI.EndChangeCheck())
                autoTrack.boolValue = track;

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(80f)))
            {
                _serializedConfig.ApplyModifiedProperties();
                CollectibleDatabaseTracker.Refresh(_config);
                _serializedConfig.Update();
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawGroups()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width * ListWidthRatio), GUILayout.ExpandWidth(false));
            _listScroll = EditorGUILayout.BeginScrollView(_listScroll);

            for (int i = 0; i < Groups.Length; i++)
                DrawGroup(Groups[i]);

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawGroup(GroupDefinition group)
        {
            SerializedProperty list = _serializedConfig.FindProperty(group.PropertyName);
            if (list == null)
                return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{group.Title} ({list.arraySize})", EditorStyles.boldLabel);
            if (GUILayout.Button("+", GUILayout.Width(22f)))
            {
                CreateAndAdd(group);
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndHorizontal();

            if (list.arraySize == 0)
                EditorGUILayout.LabelField("Kayıt yok", EditorStyles.miniLabel);

            for (int i = 0; i < list.arraySize; i++)
                DrawRow(list, i);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4f);
        }

        private void DrawRow(SerializedProperty list, int index)
        {
            var collectible = list.GetArrayElementAtIndex(index).objectReferenceValue as BaseCollectibleData;
            Rect rowRect = EditorGUILayout.GetControlRect(false, RowHeight);

            if (Event.current.type == EventType.Repaint && collectible != null && collectible == _selected)
                EditorGUI.DrawRect(rowRect, new Color(0.24f, 0.48f, 0.9f, 0.35f));

            Rect iconRect = new Rect(rowRect.x + 4f, rowRect.y + 3f, IconSize, IconSize);
            Rect removeRect = new Rect(rowRect.xMax - RemoveSize - 2f, rowRect.y + 1f, RemoveSize, RowHeight - 2f);
            Rect idRect = new Rect(removeRect.x - IdWidth - 4f, rowRect.y, IdWidth, rowRect.height);
            Rect nameRect = new Rect(iconRect.xMax + 6f, rowRect.y, idRect.x - iconRect.xMax - 10f, rowRect.height);

            if (GUI.Button(removeRect, "-"))
            {
                bool removingSelected = collectible != null && collectible == _selected;
                CollectibleDatabaseTracker.RemoveAt(list, index);
                _serializedConfig.ApplyModifiedProperties();
                if (removingSelected)
                {
                    _selected = null;
                    _selectedSerialized = null;
                }

                GUIUtility.ExitGUI();
            }

            if (collectible == null)
            {
                EditorGUI.LabelField(nameRect, "Missing reference");
                return;
            }

            if (collectible.Icon != null)
                DatabaseEditorUtility.DrawSprite(iconRect, collectible.Icon);

            string label = string.IsNullOrEmpty(collectible.DisplayName) ? collectible.name : collectible.DisplayName;
            EditorGUI.LabelField(nameRect, label);
            EditorGUI.LabelField(idRect, collectible.ItemId, EditorStyles.miniLabel);

            var selectRect = new Rect(rowRect.x, rowRect.y, removeRect.x - rowRect.x, rowRect.height);
            if (GUI.Button(selectRect, GUIContent.none, GUIStyle.none))
            {
                Select(collectible);
                GUIUtility.ExitGUI();
            }
        }

        private void DrawDetail()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandWidth(true));
            _detailScroll = EditorGUILayout.BeginScrollView(_detailScroll);

            if (_selected == null)
            {
                _selectedSerialized = null;
                EditorGUILayout.HelpBox("Ayrıntı için bir satır seç.", MessageType.Info);
            }
            else
            {
                EnsureSelectedSerializedObject();
                _selectedSerialized.Update();
                DrawSelectedEditor();
                _selectedSerialized.ApplyModifiedProperties();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawSelectedEditor()
        {
            string title = string.IsNullOrEmpty(_selected.DisplayName) ? _selected.name : _selected.DisplayName;
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.ObjectField("Asset", _selected, typeof(BaseCollectibleData), false);

            EditorGUILayout.Space(6f);
            EditorGUILayout.BeginHorizontal();
            DrawIconColumn(_selectedSerialized.FindProperty("icon"));

            EditorGUILayout.BeginVertical();
            float previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 110f;
            DatabaseEditorUtility.DrawPropertiesExcept(_selectedSerialized, "icon");
            EditorGUIUtility.labelWidth = previousLabelWidth;

            if (GUILayout.Button("Reset", GUILayout.Height(24f)))
                ResetSelected();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawIconColumn(SerializedProperty iconProperty)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(DetailIconSize + 8f));
            Rect previewRect = GUILayoutUtility.GetRect(DetailIconSize, DetailIconSize, GUILayout.ExpandWidth(false));
            GUI.Box(previewRect, GUIContent.none, EditorStyles.helpBox);

            if (iconProperty != null && iconProperty.objectReferenceValue is Sprite sprite)
            {
                var innerRect = new Rect(previewRect.x + 8f, previewRect.y + 8f, previewRect.width - 16f, previewRect.height - 16f);
                DatabaseEditorUtility.DrawSprite(innerRect, sprite);
            }
            else
            {
                GUI.Label(previewRect, "No Icon", EditorStyles.centeredGreyMiniLabel);
            }

            if (iconProperty != null)
                EditorGUILayout.PropertyField(iconProperty, GUIContent.none);

            EditorGUILayout.EndVertical();
        }

        private void ResetSelected()
        {
            if (_selected is not CustomScriptableObject scriptable)
                return;

            Undo.RecordObject(_selected, "Reset Collectible");
            scriptable.Reset();
            EditorUtility.SetDirty(_selected);
            _selectedSerialized.Update();
        }

        private void CreateAndAdd(GroupDefinition group)
        {
            DatabaseEditorUtility.EnsureFolder(group.Folder);
            var asset = (BaseCollectibleData)ScriptableObject.CreateInstance(group.AssetType);
            string path = AssetDatabase.GenerateUniqueAssetPath($"{group.Folder}/{group.FileName}.asset");
            AssetDatabase.CreateAsset(asset, path);

            _serializedConfig.Update();
            SerializedProperty list = _serializedConfig.FindProperty(group.PropertyName);
            CollectibleDatabaseTracker.AddIfMissing(list, asset);
            _serializedConfig.ApplyModifiedProperties();
            Select(asset);
        }

        private void Select(BaseCollectibleData collectible)
        {
            if (_selected != collectible)
            {
                _selected = collectible;
                _selectedSerialized = null;
                EnsureSelectedSerializedObject();
            }

            if (collectible != null)
                EditorGUIUtility.PingObject(collectible);
        }

        private void EnsureSerializedObject()
        {
            if (_config == null)
            {
                _serializedConfig = null;
                return;
            }

            if (_serializedConfig == null || _serializedConfig.targetObject != _config)
                _serializedConfig = new SerializedObject(_config);
        }

        private void EnsureSelectedSerializedObject()
        {
            if (_selected == null)
            {
                _selectedSerialized = null;
                return;
            }

            if (_selectedSerialized == null || _selectedSerialized.targetObject != _selected)
                _selectedSerialized = new SerializedObject(_selected);
        }

        private readonly struct GroupDefinition
        {
            public GroupDefinition(string title, string propertyName, string folder, string fileName, System.Type assetType)
            {
                Title = title;
                PropertyName = propertyName;
                Folder = folder;
                FileName = fileName;
                AssetType = assetType;
            }

            public string Title { get; }
            public string PropertyName { get; }
            public string Folder { get; }
            public string FileName { get; }
            public System.Type AssetType { get; }
        }
    }
}
