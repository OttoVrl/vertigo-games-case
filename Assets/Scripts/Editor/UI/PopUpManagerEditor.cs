using System.Collections.Generic;
using Runtime.UI;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Editor.UI
{
    [CustomEditor(typeof(PopUpManager))]
    public class PopUpManagerEditor : UnityEditor.Editor
    {
        private const float Spacing = 6f;

        private float HeaderLeadingOffset = 16f;
        private const float HeaderTrailingOffset = 14f;

        private ReorderableList _popUpList;
        private SerializedProperty _popUpPairs;

        private void OnEnable()
        {
            _popUpPairs = serializedObject.FindProperty("popUpPairs");
            _popUpList = new ReorderableList(serializedObject, _popUpPairs, true, true, true, true)
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

            DrawPropertiesExcluding(serializedObject, "m_Script", "popUpPairs");
            _popUpList.DoLayoutList();
            DrawValidation();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(Rect rect)
        {
            rect.x += HeaderLeadingOffset;

            rect.width = Mathf.Max(0f, rect.width - HeaderLeadingOffset - HeaderTrailingOffset);
            Split(rect, out Rect stateRect, out Rect popUpRect);

            EditorGUI.LabelField(stateRect, "PopUp State", EditorStyles.miniBoldLabel);
            EditorGUI.LabelField(popUpRect, "PopUp", EditorStyles.miniBoldLabel);
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = _popUpPairs.GetArrayElementAtIndex(index);
            rect.y += 2f;
            rect.height = EditorGUIUtility.singleLineHeight;
            Split(rect, out Rect stateRect, out Rect popUpRect);

            int previousIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            EditorGUI.PropertyField(stateRect, element.FindPropertyRelative("popUpState"), GUIContent.none);
            EditorGUI.PropertyField(popUpRect, element.FindPropertyRelative("popUp"), GUIContent.none);

            EditorGUI.indentLevel = previousIndent;
        }

        private static void Split(Rect rect, out Rect left, out Rect right)
        {
            float half = (rect.width - Spacing) * 0.5f;
            left = new Rect(rect.x, rect.y, half, rect.height);
            right = new Rect(rect.x + half + Spacing, rect.y, half, rect.height);
        }

        private void DrawValidation()
        {
            var states = new HashSet<int>();
            bool hasDuplicate = false;

            for (int i = 0; i < _popUpPairs.arraySize; i++)
            {
                SerializedProperty element = _popUpPairs.GetArrayElementAtIndex(i);
                if (!states.Add(element.FindPropertyRelative("popUpState").enumValueIndex))
                    hasDuplicate = true;
            }

            if (hasDuplicate)
            {
                EditorGUILayout.HelpBox(
                    "Aynı PopUp State birden fazla satırda seçili",
                    MessageType.Error);
            }
        }
    }
}
