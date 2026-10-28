using UnityEditor;
using UnityEngine;

namespace Editor.Data
{
    [InitializeOnLoad]
    public class VertigoCaseWelcomeWindow : EditorWindow
    {
        private const string SessionKey = "VertigoCaseWelcomeShown";
        private const string FilePath = "Assets/Scripts/Editor/Window/VertigoCaseWelcomeWindow.cs";

        static VertigoCaseWelcomeWindow()
        {
            EditorApplication.delayCall += OpenOnce;
        }

        private static void OpenOnce()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (SessionState.GetBool(SessionKey, false)) return;

            SessionState.SetBool(SessionKey, true);
            GetWindow<VertigoCaseWelcomeWindow>(true, "Vertigo Games Case", true);
        }

        private void OnEnable()
        {
            minSize = new Vector2(480f, 440f);
        }

        private void OnGUI()
        {
            GUILayout.Space(18f);

            GUIStyle title = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 28,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };
            GUILayout.Label("VERTIGO GAMES CASE", title);

            GUILayout.Space(8f);
            GUIStyle name = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter
            };
            GUILayout.Label("OSMAN TURAN VAROL", name);

            GUILayout.Space(20f);
            EditorGUILayout.HelpBox("GameDatabaseConfig için Tools/Vertigo/Database Window penceresini aç.", MessageType.Info);
            if (GUILayout.Button("Open Window", GUILayout.Height(28f)))
                EditorApplication.ExecuteMenuItem("Tools/Vertigo/Database Window");

            GUILayout.Space(10f);
            EditorGUILayout.HelpBox("SpinDatabaseConfig için Tools/Vertigo/Spin Database Window penceresini aç.", MessageType.Info);
            if (GUILayout.Button("Open Window", GUILayout.Height(28f)))
                EditorApplication.ExecuteMenuItem("Tools/Vertigo/Spin Database Window");

            GUILayout.Space(16f);
            Color previous = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.45f, 0.82f, 0.45f);
            EditorGUILayout.HelpBox("Case de editor scripting kısımlarında ai desteği alınmıştır.", MessageType.None);
            GUI.backgroundColor = previous;

            GUILayout.Space(8f);
            EditorGUILayout.HelpBox(
                "Bu pencerenin bir daha çıkmaması için şu dosyayı silin:\n" + FilePath,
                MessageType.Warning);
        }
    }
}
