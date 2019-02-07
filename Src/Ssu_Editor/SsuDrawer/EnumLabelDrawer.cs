using System.Collections.Generic;
using System.Linq;
using Ssu.SsuAttribute;
using UnityEditor;
using UnityEngine;

namespace Ssu_Editor.SsuDrawer {
    [CustomPropertyDrawer(typeof(EnumLabelAttribute))]
    public class EnumLabelDrawer : PropertyDrawer {
        private Dictionary<string, string> customEnumNames = new Dictionary<string, string>();
        private EnumLabelAttribute enumLabelAttribute {
            get {
                return (EnumLabelAttribute)attribute;
            }
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            SetUpCustomEnumNames(property, property.enumNames);

            if (property.propertyType == SerializedPropertyType.Enum) {
                EditorGUI.BeginChangeCheck();

                var displayedOptions = (from enumName in property.enumNames
                                        where customEnumNames.ContainsKey(enumName)
                                        select customEnumNames[enumName]).ToArray();

                var selectedIndex = EditorGUI.Popup(position, enumLabelAttribute.label, property.enumValueIndex,
                    displayedOptions);
                if (EditorGUI.EndChangeCheck()) {
                    property.enumValueIndex = selectedIndex;
                }
            }
        }

        public void SetUpCustomEnumNames(SerializedProperty property, string[] enumNames) {
            // 获得目标类类型
            var type = property.serializedObject.targetObject.GetType();
            // 获得字段
            foreach (var fieldInfo in type.GetFields()) {
                // 获得自定义属性字段
                var customAttributes = fieldInfo.GetCustomAttributes(typeof(EnumLabelAttribute), false);
                foreach (var customAttribute in customAttributes) {
                    // 获得字段类型
                    var enumType = fieldInfo.FieldType;
                    foreach (var enumName in enumNames) {
                        // 获得指定名字的字段
                        var field = enumType.GetField(enumName);
                        if (field == null) {
                            continue;
                        }
                        // 获得自定义属性字段
                        var attrs = (EnumLabelAttribute[])field.GetCustomAttributes(customAttribute.GetType(), false);

                        if (!customEnumNames.ContainsKey(enumName)) {
                            foreach (var labelAttribute in attrs) {
                                customEnumNames.Add(enumName, labelAttribute.label);
                            }
                        }
                    }
                }
            }
        }
    }
}
