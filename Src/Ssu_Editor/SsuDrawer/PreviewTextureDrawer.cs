using System.IO;
using Ssu.SsuAttribute;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Ssu_Editor.SsuDrawer {
    [CustomPropertyDrawer(typeof(PreviewTextureAttribute))]
    public class PreviewTextureDrawer : PropertyDrawer {

        private GUIStyle style;

        PreviewTextureAttribute TextureAttribute {
            get {
                return (PreviewTextureAttribute)attribute;
            }
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            position.height = 16;
            if (property.propertyType == SerializedPropertyType.String) {
                DrawStringValue(position, property, label);
            } else if (property.propertyType == SerializedPropertyType.ObjectReference) {
                DrawTextureValue(position, property, label);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return base.GetPropertyHeight(property, label) + TextureAttribute.lastPosition.height;
        }

        void DrawTextureValue(Rect position, SerializedProperty property, GUIContent label) {
            property.objectReferenceValue = (Texture2D)EditorGUI.ObjectField(position, label,
                property.objectReferenceValue, typeof(Texture2D), false);

            if (property.objectReferenceValue != null) {
                DrawTexture(position, (Texture2D)property.objectReferenceValue);
            }
        }

        void DrawStringValue(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginChangeCheck();
            property.stringValue = EditorGUI.TextField(position, label, property.stringValue);
            if (EditorGUI.EndChangeCheck()) {
                TextureAttribute.webRequest = null;
                TextureAttribute.texture = null;
            }
            var path = GetCachedTexturePath(property.stringValue);

            if (!string.IsNullOrEmpty(path)) {
                if (IsExpired(path)) {
                    Delete(path);
                } else if (TextureAttribute.texture == null) {
                    TextureAttribute.texture = GetTextureFromCached(path);
                }
            } else {
                TextureAttribute.texture = null;
            }

            if (TextureAttribute.texture == null) {
                TextureAttribute.texture = GetTextureFromWeb(position, property);
            } else {
                DrawTexture(position, TextureAttribute.texture);
            }
        }
        // 是否过期
        bool IsExpired(string path) {
            var fileName = Path.GetFileNameWithoutExtension(path);
            var split = fileName.Split('_');
            return System.DateTime.Now.Ticks >= long.Parse(split[1]);
        }
        // 从缓存获得纹理路径
        string GetCachedTexturePath(string stringValue) {
            var hash = stringValue.GetHashCode();
            foreach (var path in Directory.GetFiles("Temp")) {
                if (Path.GetFileNameWithoutExtension(path).StartsWith(hash.ToString())) {
                    return path;
                }
            }
            return string.Empty;
        }
        // 从Web获得纹理
        Texture2D GetTextureFromWeb(Rect position, SerializedProperty property) {
            if (TextureAttribute.webRequest == null) {
                TextureAttribute.webRequest = UnityWebRequestTexture.GetTexture(property.stringValue);
            } else if (!TextureAttribute.webRequest.isDone) {
                TextureAttribute.webRequest.Send();
                TextureAttribute.lastPosition = new Rect(position.x, position.y + 16, position.width, 16);
                var progress = TextureAttribute.webRequest.downloadProgress;
                EditorGUI.ProgressBar(TextureAttribute.lastPosition, progress, "Downloading... " + (progress * 100) + "%");
            } else if (TextureAttribute.webRequest.isDone) {
                if (TextureAttribute.webRequest.isNetworkError) {
                    return null;
                }
                var hash = property.stringValue.GetHashCode();
                var expire = (System.DateTime.Now.Ticks + TextureAttribute.expire);
                var texture = DownloadHandlerTexture.GetContent(TextureAttribute.webRequest);
                File.WriteAllBytes(string.Format("Temp/{0}_{1}_{2}_{3}", hash, expire, texture.width, texture.height),
                    TextureAttribute.webRequest.downloadHandler.data);
                return texture;
            }
            return null;
        }
        // 从缓存获得纹理
        Texture2D GetTextureFromCached(string path) {
            var split = Path.GetFileNameWithoutExtension(path).Split('_');
            var width = int.Parse(split[2]);
            var height = int.Parse(split[3]);
            var texture = new Texture2D(width, height);

            return texture.LoadImage(File.ReadAllBytes(path)) ? texture : null;
        }

        void DrawTexture(Rect position, Texture2D texture) {
            var width = Mathf.Clamp(texture.width, position.width * 0.7f, position.width * 0.7f);
            TextureAttribute.lastPosition = new Rect(position.width * 0.15f, position.y + 16, width,
                texture.height * (width / texture.width));

            if (style == null) {
                style = new GUIStyle();
                style.imagePosition = ImagePosition.ImageOnly;
            }
            style.normal.background = texture;
            GUI.Label(TextureAttribute.lastPosition, "", style);
        }

        void Delete(string path) {
            File.Delete(path);
            TextureAttribute.texture = null;
        }
    }
}
