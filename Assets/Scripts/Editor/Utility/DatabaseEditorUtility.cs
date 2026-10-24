using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor.Utility
{
    public static class DatabaseEditorUtility
    {
        public static T CreateAsset<T>(string folder, string fileName, Action<SerializedObject> initialize = null)
            where T : ScriptableObject
        {
            EnsureFolder(folder);

            var asset = ScriptableObject.CreateInstance<T>();
            if (initialize != null)
            {
                var serializedAsset = new SerializedObject(asset);
                initialize(serializedAsset);
                serializedAsset.ApplyModifiedPropertiesWithoutUndo();
            }

            string path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{fileName}.asset");
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        public static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
                return;

            string parent = Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
            string folderName = Path.GetFileName(folderPath);
            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
                EnsureFolder(parent);

            AssetDatabase.CreateFolder(parent, folderName);
        }

        public static void DrawPropertiesExcept(SerializedObject serializedObject, params string[] excludedNames)
        {
            SerializedProperty property = serializedObject.GetIterator();
            bool enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (property.name == "m_Script" || Array.IndexOf(excludedNames, property.name) >= 0)
                    continue;

                EditorGUILayout.PropertyField(property.Copy(), true);
            }
        }

        public static void DrawSprite(Rect rect, Sprite sprite)
        {
            Texture2D texture = sprite.texture;
            if (texture == null || texture.width == 0 || texture.height == 0)
                return;

            Rect textureRect = sprite.textureRect;
            float spriteAspect = textureRect.height <= 0f ? 1f : textureRect.width / textureRect.height;
            float rectAspect = rect.height <= 0f ? 1f : rect.width / rect.height;
            Rect fitted;
            if (spriteAspect > rectAspect)
            {
                float height = rect.width / spriteAspect;
                fitted = new Rect(rect.x, rect.y + (rect.height - height) * 0.5f, rect.width, height);
            }
            else
            {
                float width = rect.height * spriteAspect;
                fitted = new Rect(rect.x + (rect.width - width) * 0.5f, rect.y, width, rect.height);
            }

            var uv = new Rect(
                textureRect.x / texture.width,
                textureRect.y / texture.height,
                textureRect.width / texture.width,
                textureRect.height / texture.height);
            GUI.DrawTextureWithTexCoords(fitted, texture, uv, true);
        }
    }
}
