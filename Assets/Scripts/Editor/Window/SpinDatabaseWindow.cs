using System.Collections.Generic;
using Editor.Utility;
using Runtime.Data;
using UnityEditor;
using UnityEngine;

namespace Editor.Data
{
    public class SpinDatabaseWindow : EditorWindow
    {
        private const string RootFolder = "Assets/Datas/MetaDatas/SpinModeDatas";
        private const string ZoneFolder = RootFolder + "/Zones";
        private const string WheelFolder = RootFolder + "/Wheels";
        private const string RewardPoolFolder = RootFolder + "/RewardPools";
        private const string ZoneListProperty = "zoneDatas";

        private const float RowHeight = 22f;
        private const float IconSize = 16f;
        private const float RemoveSize = 20f;
        private const float InfoWidth = 90f;
        private const float ListWidthRatio = 0.3f;
        private const float PreviewSize = 128f;

        private static readonly Color SelectedRowColor = new Color(0.24f, 0.48f, 0.9f, 0.35f);

        [SerializeField] private SpinDatabaseConfig _config;
        [SerializeField] private ScriptableObject _selected;

        private readonly List<WheelConfig> _wheels = new List<WheelConfig>();
        private readonly List<RewardPool> _rewardPools = new List<RewardPool>();

        private SerializedObject _serializedConfig;
        private SerializedObject _selectedSerialized;
        private Vector2 _listScroll;
        private Vector2 _detailScroll;
        private bool _showSettings = true;

        [MenuItem("Tools/Vertigo/Spin Database Window")]
        private static void OpenFromMenu()
        {
            var window = GetWindow<SpinDatabaseWindow>("Spin Database");
            SpinDatabaseConfig config = Selection.activeObject as SpinDatabaseConfig;
            if (config == null)
                config = window._config;

            if (config == null)
            {
                string[] guids = AssetDatabase.FindAssets($"t:{nameof(SpinDatabaseConfig)}");
                if (guids.Length > 0)
                    config = AssetDatabase.LoadAssetAtPath<SpinDatabaseConfig>(AssetDatabase.GUIDToAssetPath(guids[0]));
            }

            if (config != null)
                Open(config);
            else
                window.Show();
        }

        public static void Open(SpinDatabaseConfig config)
        {
            var window = GetWindow<SpinDatabaseWindow>("Spin Database");
            if (window._config != config)
            {
                window._config = config;
                window.ClearSelection();
            }

            window.EnsureSerializedObject();
            window.RefreshProjectAssets();
            window.Show();
            window.Focus();
        }

        private void OnEnable()
        {
            minSize = new Vector2(760f, 400f);
            titleContent = new GUIContent("Spin Database");
            EnsureSerializedObject();
            RefreshProjectAssets();
        }

        private void OnProjectChange()
        {
            RefreshProjectAssets();
            Repaint();
        }

        private void OnGUI()
        {
            EnsureSerializedObject();

            if (_config == null || _serializedConfig == null)
            {
                EditorGUILayout.HelpBox(
                    "SpinModeConfig seçili değil. Tools/Vertigo/Spin Database Window menüsünü veya asset inspector'ındaki Open Spin Database Window butonunu kullan.",
                    MessageType.Info);
                return;
            }

            _serializedConfig.Update();
            DrawToolbar();

            EditorGUILayout.BeginHorizontal();
            DrawLeftPanel();
            DrawDetail();
            EditorGUILayout.EndHorizontal();

            _serializedConfig.ApplyModifiedProperties();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label(_config.name, EditorStyles.miniLabel);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Ping Config", EditorStyles.toolbarButton, GUILayout.Width(80f)))
                EditorGUIUtility.PingObject(_config);

            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(70f)))
            {
                RemoveMissingZones();
                RefreshProjectAssets();
                ApplyAndExit();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawLeftPanel()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width * ListWidthRatio), GUILayout.ExpandWidth(false));
            _listScroll = EditorGUILayout.BeginScrollView(_listScroll);

            DrawSettings();
            DrawZoneGroup();
            DrawWheelGroup();
            DrawRewardPoolGroup();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawSettings()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _showSettings = EditorGUILayout.Foldout(_showSettings, "Settings", true);

            if (_showSettings)
            {
                float previousLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 130f;
                DrawConfigField("zoneStepCount");
                DrawConfigField("rewardIncreasePerZone");
                EditorGUILayout.Space(6f);
                DrawReviveSettings();
                EditorGUILayout.Space(6f);
                DrawStartTypes();
                EditorGUIUtility.labelWidth = previousLabelWidth;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4f);
        }

        private void DrawConfigField(string propertyName)
        {
            SerializedProperty property = _serializedConfig.FindProperty(propertyName);
            if (property != null)
                EditorGUILayout.PropertyField(property);
        }

        private void DrawReviveSettings()
        {
            EditorGUILayout.LabelField("Revive", EditorStyles.boldLabel);
            DrawConfigField("startReviveCost", "Start Cost");
            DrawConfigField("reviveCostIncreasePerZone", "Increase / Zone");
            DrawConfigField("reviveCostCurrencyType", "Currency");

            SerializedProperty startCost = _serializedConfig.FindProperty("startReviveCost");
            SerializedProperty increase = _serializedConfig.FindProperty("reviveCostIncreasePerZone");
            SerializedProperty currencyProperty = _serializedConfig.FindProperty("reviveCostCurrencyType");
            SerializedProperty zoneStepCount = _serializedConfig.FindProperty("zoneStepCount");
            if (startCost == null || increase == null)
                return;

            if (startCost.intValue < 0)
                EditorGUILayout.HelpBox("Revive başlangıç maliyeti 0'dan küçük olamaz.", MessageType.Warning);

            if (currencyProperty != null && currencyProperty.objectReferenceValue == null)
                EditorGUILayout.HelpBox("Revive currency seçilmedi.", MessageType.Warning);

            int lastZone = Mathf.Max(1, zoneStepCount != null ? zoneStepCount.intValue : 1);
            EditorGUILayout.LabelField("Örnek maliyet", EditorStyles.miniBoldLabel);
            DrawReviveSample(1, startCost.intValue, increase.floatValue);
            DrawReviveSample(Mathf.Clamp(5, 1, lastZone), startCost.intValue, increase.floatValue);
            DrawReviveSample(lastZone, startCost.intValue, increase.floatValue);
        }

        private static void DrawReviveSample(int zone, int startCost, float increasePerZone)
        {
            float multiplier = 1f + (zone - 1) * increasePerZone;
            int cost = Mathf.Max(0, Mathf.RoundToInt(startCost * multiplier));
            EditorGUILayout.LabelField($"Zone {zone}  →  {cost}", EditorStyles.miniLabel);
        }

        private void DrawConfigField(string propertyName, string label)
        {
            SerializedProperty property = _serializedConfig.FindProperty(propertyName);
            if (property != null)
                EditorGUILayout.PropertyField(property, new GUIContent(label));
        }

        private void DrawStartTypes()
        {
            SerializedProperty list = _serializedConfig.FindProperty("customStartTypes");
            if (list == null)
                return;

            EditorGUILayout.LabelField("Start Types", EditorStyles.boldLabel);

            if (list.arraySize == 0)
                EditorGUILayout.LabelField("Kayıt yok", EditorStyles.miniLabel);

            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty element = list.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Start {i + 1}", EditorStyles.miniBoldLabel);
                bool remove = GUILayout.Button("-", GUILayout.Width(22f));
                EditorGUILayout.EndHorizontal();

                if (remove)
                {
                    list.DeleteArrayElementAtIndex(i);
                    ApplyAndExit();
                }

                DrawElementField(element, "_targetCurrencyData", "Currency");
                DrawElementField(element, "cost", "Cost");
                DrawElementField(element, "startZoneIndex", "Start Zone");

                SerializedProperty currency = element.FindPropertyRelative("_targetCurrencyData");
                SerializedProperty startZone = element.FindPropertyRelative("startZoneIndex");
                if ((currency != null && currency.objectReferenceValue == null) || (startZone != null && startZone.intValue < 1))
                    EditorGUILayout.HelpBox("Currency seç ve başlangıç zone'unu 1 veya daha büyük yap.", MessageType.Warning);

                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("+ Start Type"))
            {
                int index = list.arraySize;
                list.InsertArrayElementAtIndex(index);
                SerializedProperty element = list.GetArrayElementAtIndex(index);
                SerializedProperty currency = element.FindPropertyRelative("_targetCurrencyData");
                SerializedProperty cost = element.FindPropertyRelative("cost");
                SerializedProperty startZone = element.FindPropertyRelative("startZoneIndex");
                if (currency != null)
                    currency.objectReferenceValue = null;
                if (cost != null)
                    cost.intValue = 0;
                if (startZone != null)
                    startZone.intValue = 1;

                ApplyAndExit();
            }
        }

        private static void DrawElementField(SerializedProperty element, string relativeName, string label)
        {
            SerializedProperty property = element.FindPropertyRelative(relativeName);
            if (property != null)
                EditorGUILayout.PropertyField(property, new GUIContent(label));
        }

        private void DrawZoneGroup()
        {
            SerializedProperty list = _serializedConfig.FindProperty(ZoneListProperty);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (list == null)
            {
                EditorGUILayout.LabelField($"{ZoneListProperty} alanı bulunamadı", EditorStyles.miniLabel);
                EditorGUILayout.EndVertical();
                return;
            }

            if (DrawGroupHeader("Zones", list.arraySize))
            {
                CreateZone(list);
                ApplyAndExit();
            }

            if (list.arraySize == 0)
                EditorGUILayout.LabelField("Kayıt yok", EditorStyles.miniLabel);

            for (int i = 0; i < list.arraySize; i++)
            {
                var zone = list.GetArrayElementAtIndex(i).objectReferenceValue as ZoneData;
                string label = zone ? GetDisplayName(zone) : null;
                string info = zone ? $"every {zone.StepInterval}" : string.Empty;
                Sprite icon = zone && zone.WheelConfig ? zone.WheelConfig.WheelSprite : null;
                Color? swatch = zone ? zone.ZoneColor : (Color?)null;

                if (DrawRow(zone, label, info, icon, swatch, true))
                {
                    if (zone && zone == _selected)
                        ClearSelection();

                    list.DeleteArrayElementAtIndex(i);
                    ApplyAndExit();
                }
            }

            DrawZoneValidation(list);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4f);
        }

        private void DrawZoneValidation(SerializedProperty list)
        {
            var intervals = new HashSet<int>();
            bool hasDefaultZone = false;
            bool hasDuplicateInterval = false;

            for (int i = 0; i < list.arraySize; i++)
            {
                var zone = list.GetArrayElementAtIndex(i).objectReferenceValue as ZoneData;
                if (!zone)
                    continue;

                if (zone.StepInterval == 1)
                    hasDefaultZone = true;

                if (!intervals.Add(zone.StepInterval))
                    hasDuplicateInterval = true;
            }

            if (list.arraySize > 0 && !hasDefaultZone)
                EditorGUILayout.HelpBox("Aralığı 1 olan zone yok; bazı zone'larda çark bulunamaz.", MessageType.Warning);

            if (hasDuplicateInterval)
                EditorGUILayout.HelpBox("Aynı aralıkta birden fazla zone var; hangisinin eşleşeceği belirsiz.", MessageType.Warning);
        }

        private void DrawWheelGroup()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (DrawGroupHeader("Wheels", _wheels.Count))
            {
                Select(CreateWheel("WheelConfig"));
                ApplyAndExit();
            }

            if (_wheels.Count == 0)
                EditorGUILayout.LabelField("Kayıt yok", EditorStyles.miniLabel);

            foreach (var wheel in _wheels)
            {
                if (!wheel)
                    continue;

                string info = wheel.HasBomb ? $"{wheel.Slices.Count} slice, bomb" : $"{wheel.Slices.Count} slice";
                DrawRow(wheel, wheel.name, info, wheel.WheelSprite, null, false);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4f);
        }

        private void DrawRewardPoolGroup()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (DrawGroupHeader("Reward Pools", _rewardPools.Count))
            {
                Select(CreateRewardPool());
                ApplyAndExit();
            }

            if (_rewardPools.Count == 0)
                EditorGUILayout.LabelField("Kayıt yok", EditorStyles.miniLabel);

            foreach (var rewardPool in _rewardPools)
            {
                if (rewardPool)
                    DrawRow(rewardPool, rewardPool.name, string.Empty, null, null, false);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4f);
        }

        private static bool DrawGroupHeader(string title, int count)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{title} ({count})", EditorStyles.boldLabel);
            bool addClicked = GUILayout.Button("+", GUILayout.Width(22f));
            EditorGUILayout.EndHorizontal();
            return addClicked;
        }

        private bool DrawRow(ScriptableObject item, string label, string info, Sprite icon, Color? swatch, bool removable)
        {
            Rect rowRect = EditorGUILayout.GetControlRect(false, RowHeight);

            if (Event.current.type == EventType.Repaint && item && item == _selected)
                EditorGUI.DrawRect(rowRect, SelectedRowColor);

            var iconRect = new Rect(rowRect.x + 4f, rowRect.y + 3f, IconSize, IconSize);
            float rightEdge = rowRect.xMax;
            Rect removeRect = default;
            if (removable)
            {
                removeRect = new Rect(rowRect.xMax - RemoveSize - 2f, rowRect.y + 1f, RemoveSize, RowHeight - 2f);
                rightEdge = removeRect.x - 4f;
            }

            var infoRect = new Rect(rightEdge - InfoWidth, rowRect.y, InfoWidth, rowRect.height);
            var nameRect = new Rect(iconRect.xMax + 6f, rowRect.y, infoRect.x - iconRect.xMax - 10f, rowRect.height);

            if (removable && GUI.Button(removeRect, "-"))
                return true;

            if (!item)
            {
                EditorGUI.LabelField(nameRect, "Missing reference");
                return false;
            }

            if (swatch.HasValue)
                EditorGUI.DrawRect(iconRect, swatch.Value);

            if (icon)
                DatabaseEditorUtility.DrawSprite(iconRect, icon);

            EditorGUI.LabelField(nameRect, label);
            EditorGUI.LabelField(infoRect, info, EditorStyles.miniLabel);

            var selectRect = new Rect(rowRect.x, rowRect.y, rightEdge - rowRect.x, rowRect.height);
            if (GUI.Button(selectRect, GUIContent.none, GUIStyle.none))
            {
                Select(item);
                ApplyAndExit();
            }

            return false;
        }

        private void DrawDetail()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandWidth(true));
            _detailScroll = EditorGUILayout.BeginScrollView(_detailScroll);

            if (!_selected)
            {
                ClearSelection();
                EditorGUILayout.HelpBox("Ayrıntı için bir satır seç.", MessageType.Info);
            }
            else
            {
                EnsureSelectedSerializedObject();
                _selectedSerialized.Update();

                EditorGUILayout.LabelField(GetDisplayName(_selected), EditorStyles.boldLabel);
                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.ObjectField("Asset", _selected, typeof(ScriptableObject), false);
                EditorGUILayout.Space(6f);

                switch (_selected)
                {
                    case ZoneData zone:
                        DrawZoneDetail(zone);
                        break;
                    case WheelConfig wheel:
                        DrawWheelDetail(wheel);
                        break;
                    default:
                        DatabaseEditorUtility.DrawPropertiesExcept(_selectedSerialized);
                        break;
                }

                bool changed = _selectedSerialized.ApplyModifiedProperties();
                if (changed && _selected is ZoneData)
                    SortZoneList();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawZoneDetail(ZoneData zone)
        {
            SerializedProperty colorProperty = _selectedSerialized.FindProperty("zoneColor");
            SerializedProperty wheelProperty = _selectedSerialized.FindProperty("wheelConfig");
            var wheel = wheelProperty?.objectReferenceValue as WheelConfig;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(PreviewSize + 8f));
            Color background = colorProperty != null ? colorProperty.colorValue : Color.clear;
            DrawPreview(wheel ? wheel.WheelSprite : null, background, "No Wheel");
            if (colorProperty != null)
                EditorGUILayout.PropertyField(colorProperty, GUIContent.none);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            float previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 110f;
            DatabaseEditorUtility.DrawPropertiesExcept(_selectedSerialized, "zoneColor");
            EditorGUIUtility.labelWidth = previousLabelWidth;

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Safe Zone", zone.IsSafe ? "Evet, oyuncu ayrılabilir" : "Hayır, çarkta bomba var");

            if (wheel)
            {
                if (GUILayout.Button("Select Wheel", GUILayout.Height(24f)))
                {
                    Select(wheel);
                    ApplyAndExit();
                }
            }
            else if (wheelProperty != null && GUILayout.Button("Create Wheel For Zone", GUILayout.Height(24f)))
            {
                wheelProperty.objectReferenceValue = CreateWheel($"{zone.name}_Wheel");
                ApplyAndExit();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawWheelDetail(WheelConfig wheel)
        {
            SerializedProperty spriteProperty = _selectedSerialized.FindProperty("wheelSprite");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(PreviewSize + 8f));
            DrawPreview(spriteProperty?.objectReferenceValue as Sprite, Color.clear, "No Sprite");
            if (spriteProperty != null)
                EditorGUILayout.PropertyField(spriteProperty, GUIContent.none);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            float previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 110f;
            DatabaseEditorUtility.DrawPropertiesExcept(_selectedSerialized, "wheelSprite");
            EditorGUIUtility.labelWidth = previousLabelWidth;

            DrawBombWarning(wheel);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawBombWarning(WheelConfig wheel)
        {
            int bombSliceCount = 0;
            foreach (var slice in wheel.Slices)
            {
                if (slice != null && slice.IsBomb)
                    bombSliceCount++;
            }

            if (wheel.HasBomb && bombSliceCount == 0)
                EditorGUILayout.HelpBox("Has Bomb açık ama çarkta bomba dilimi yok.", MessageType.Warning);
            else if (!wheel.HasBomb && bombSliceCount > 0)
                EditorGUILayout.HelpBox("Çarkta bomba dilimi var ama Has Bomb kapalı; bu zone güvenli sayılır.", MessageType.Warning);
        }

        private static void DrawPreview(Sprite sprite, Color background, string emptyLabel)
        {
            Rect previewRect = GUILayoutUtility.GetRect(PreviewSize, PreviewSize, GUILayout.ExpandWidth(false));
            GUI.Box(previewRect, GUIContent.none, EditorStyles.helpBox);

            if (background.a > 0f)
                EditorGUI.DrawRect(new Rect(previewRect.x + 2f, previewRect.y + 2f, previewRect.width - 4f, previewRect.height - 4f), background);

            if (sprite)
            {
                var innerRect = new Rect(previewRect.x + 8f, previewRect.y + 8f, previewRect.width - 16f, previewRect.height - 16f);
                DatabaseEditorUtility.DrawSprite(innerRect, sprite);
            }
            else
            {
                GUI.Label(previewRect, emptyLabel, EditorStyles.centeredGreyMiniLabel);
            }
        }

        private void CreateZone(SerializedProperty list)
        {
            var zone = DatabaseEditorUtility.CreateAsset<ZoneData>(ZoneFolder, "ZoneData", serializedZone =>
            {
                SerializedProperty interval = serializedZone.FindProperty("stepInterval");
                if (interval != null)
                    interval.intValue = 1;

                SerializedProperty color = serializedZone.FindProperty("zoneColor");
                if (color != null)
                    color.colorValue = Color.white;
            });

            int index = list.arraySize;
            list.InsertArrayElementAtIndex(index);
            list.GetArrayElementAtIndex(index).objectReferenceValue = zone;
            SortZoneList();
            Select(zone);
        }

        private WheelConfig CreateWheel(string fileName)
        {
            var wheel = DatabaseEditorUtility.CreateAsset<WheelConfig>(WheelFolder, fileName);
            RefreshProjectAssets();
            return wheel;
        }

        private RewardPool CreateRewardPool()
        {
            var rewardPool = DatabaseEditorUtility.CreateAsset<RewardPool>(RewardPoolFolder, "RewardPool");
            RefreshProjectAssets();
            return rewardPool;
        }

        private void SortZoneList()
        {
            SerializedProperty list = _serializedConfig.FindProperty(ZoneListProperty);
            if (list == null)
                return;

            var zones = new List<ZoneData>(list.arraySize);
            for (int i = 0; i < list.arraySize; i++)
                zones.Add(list.GetArrayElementAtIndex(i).objectReferenceValue as ZoneData);

            zones.Sort((a, b) => GetInterval(b).CompareTo(GetInterval(a)));

            for (int i = 0; i < zones.Count; i++)
                list.GetArrayElementAtIndex(i).objectReferenceValue = zones[i];
        }

        private void RemoveMissingZones()
        {
            SerializedProperty list = _serializedConfig.FindProperty(ZoneListProperty);
            if (list == null)
                return;

            for (int i = list.arraySize - 1; i >= 0; i--)
            {
                if (list.GetArrayElementAtIndex(i).objectReferenceValue == null)
                    list.DeleteArrayElementAtIndex(i);
            }
        }

        private void RefreshProjectAssets()
        {
            LoadAll(_wheels);
            LoadAll(_rewardPools);
        }

        private static void LoadAll<T>(List<T> target) where T : ScriptableObject
        {
            target.Clear();
            foreach (string guid in AssetDatabase.FindAssets($"t:{typeof(T).Name}"))
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                if (asset)
                    target.Add(asset);
            }

            target.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
        }

        private void Select(ScriptableObject item)
        {
            _selectedSerialized?.ApplyModifiedProperties();

            if (_selected != item)
            {
                _selected = item;
                _selectedSerialized = null;
                EnsureSelectedSerializedObject();
            }

            if (item)
                EditorGUIUtility.PingObject(item);
        }

        private void ClearSelection()
        {
            _selected = null;
            _selectedSerialized = null;
        }

        private void ApplyAndExit()
        {
            _serializedConfig?.ApplyModifiedProperties();
            _selectedSerialized?.ApplyModifiedProperties();
            GUIUtility.ExitGUI();
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
            if (!_selected)
            {
                _selectedSerialized = null;
                return;
            }

            if (_selectedSerialized == null || _selectedSerialized.targetObject != _selected)
                _selectedSerialized = new SerializedObject(_selected);
        }

        private static int GetInterval(ZoneData zone) => zone ? zone.StepInterval : 0;

        private static string GetDisplayName(ScriptableObject item)
        {
            if (item is ZoneData zone && !string.IsNullOrEmpty(zone.DisplayName))
                return zone.DisplayName;

            return item.name;
        }
    }
}
