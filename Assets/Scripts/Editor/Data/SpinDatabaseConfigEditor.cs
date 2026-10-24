using Runtime.Data;
using UnityEditor;
using UnityEngine;

namespace Editor.Data
{
    [CustomEditor(typeof(SpinDatabaseConfig))]
    public class SpinDatabaseConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Spin Database Window", GUILayout.Height(28f)))
                SpinDatabaseWindow.Open((SpinDatabaseConfig)target);
        }
    }
}
