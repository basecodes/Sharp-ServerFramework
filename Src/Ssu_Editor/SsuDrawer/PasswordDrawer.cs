using Ssu.SsuAttribute;
using UnityEditor;
using UnityEngine;

namespace Ssu_Editor.SsuDrawer {
    [CustomPropertyDrawer(typeof(PasswordAttribute))]
    public class PasswordDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (!IsSupported(property)) {
                return;
            }

            var password = property.stringValue;
            var maxLength = passwordAttribute.maxLength;
            position.height = 16;
            if (property.stringValue.Length > maxLength) {
                password = password.Substring(0, maxLength);
            }

            if (!passwordAttribute.useMask) {
                property.stringValue = EditorGUI.TextField(position, label, password);
            } else {
                property.stringValue = EditorGUI.PasswordField(position, label, password);
            }

            if (IsValid(property)) {
                DrawHelpBox(position);
            }
        }

        void DrawHelpBox(Rect position) {
            position.x += 10;
            position.y += 20;
            position.width -= 10;
            position.height += 8;
            EditorGUI.HelpBox(position, string.Format("Password must contain at least {0} characters!", passwordAttribute.minLength), MessageType.Error);
        }

        bool IsSupported(SerializedProperty property) {
            return property.propertyType == SerializedPropertyType.String;
        }

        bool IsValid(SerializedProperty property) {
            return property.stringValue.Length < passwordAttribute.minLength;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if (IsSupported(property)) {
                if (IsValid(property)) {
                    return base.GetPropertyHeight(property, label) + 30;
                }
            }
            return base.GetPropertyHeight(property, label);
        }

        PasswordAttribute passwordAttribute {
            get {
                return (PasswordAttribute)attribute;
            }
        }
    }
}
