using System.Collections.Generic;
using Runtime.UI;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Editor.UI
{
    [CustomEditor(typeof(PanelManager))]
    public class PanelManagerEditor : UnityEditor.Editor
    {
        private const float Spacing = 6f;

        private float HeaderLeadingOffset = 16f;
        private const float HeaderTrailingOffset = 14f;

        private ReorderableList _panelList;
        private SerializedProperty _panelPairs;

        private void OnEnable()
        {
            _panelPairs = serializedObject.FindProperty("panelPairs");
            _panelList = new ReorderableList(serializedObject, _panelPairs, true, true, true, true)
            {
                drawHeaderCallback = DrawHeader,
                drawElementCallback = DrawElement,
                elementHeightCallback = GetElementHeight
            };
        }

        private float GetElementHeight(int index)
        {
            return EditorGUIUtility.singleLineHeight + 6f;
        }
       
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            }

            DrawPropertiesExcluding(serializedObject, "m_Script", "panelPairs");
            _panelList.DoLayoutList();
            DrawValidation();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(Rect rect)
        {
            rect.x += HeaderLeadingOffset;
            
            rect.width = Mathf.Max(0f, rect.width - HeaderLeadingOffset - HeaderTrailingOffset);
            Split(rect, out Rect stateRect, out Rect panelRect);

            EditorGUI.LabelField(stateRect, "Game State", EditorStyles.miniBoldLabel);
            EditorGUI.LabelField(panelRect, "Panel", EditorStyles.miniBoldLabel);
        }
        
        
        
        
        private void DrawElement(Rect rect, int index, bool isActive, bool isFocus)
        {
            SerializedProperty element = _panelPairs.GetArrayElementAtIndex(index);
            rect.y += 2f;
            rect.height = EditorGUIUtility.singleLineHeight;
            Split(rect, out Rect stateRect, out Rect panelRect);

            int previousIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            EditorGUI.PropertyField(stateRect, element.FindPropertyRelative("gameState"), GUIContent.none);
            EditorGUI.PropertyField(panelRect, element.FindPropertyRelative("panel"), GUIContent.none);

            EditorGUI.indentLevel = previousIndent;
        }

        private static void Split(Rect rect, out Rect left, out Rect rigth)
        {
            float half = (rect.width - Spacing) * 0.5f;
            left = new Rect(rect.x, rect.y, half, rect.height);
            rigth = new Rect(rect.x + half + Spacing, rect.y, half, rect.height);
        }

        
        
        private void DrawValidation()
        {
            var states = new HashSet<int>();
            bool hasDuplicate = false;
            //bool hasMissingPanel = false;

            for (int i = 0; i < _panelPairs.arraySize; i++)
            {
                SerializedProperty element = _panelPairs.GetArrayElementAtIndex(i);
                if (!states.Add(element.FindPropertyRelative("gameState").enumValueIndex))
                    hasDuplicate = true;
            }

            if (hasDuplicate)
            {
                EditorGUILayout.HelpBox(
                    "Aynı Game State birden fazla satırda seçili",
                    MessageType.Error);
            }
        }
    }
}
