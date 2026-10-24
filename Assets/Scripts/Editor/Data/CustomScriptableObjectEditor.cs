using Runtime.Data;
using Runtime.Managers;
using UnityEditor;
using UnityEngine;

namespace Editor.Data
{
    [CustomEditor(typeof(CustomScriptableObject),true)]
    public class CustomScriptableObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            var script = (CustomScriptableObject)target;

            if (GUILayout.Button("Reset"))
            {
                script.Reset();
            }
        }
    }
}
