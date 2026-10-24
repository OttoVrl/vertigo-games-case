using Runtime.Data;
using UnityEditor;
using UnityEngine;

namespace Editor.Data
{
    [CustomEditor(typeof(GameDatabaseConfig))]
    public class GameDatabaseConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Database Window", GUILayout.Height(28f)))
                GameDatabaseWindow.Open((GameDatabaseConfig)target);
        }
    }
}
