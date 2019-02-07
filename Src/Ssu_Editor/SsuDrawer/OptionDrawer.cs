using System;
using System.Collections.Generic;
using Ssu.SsuAttribute;
using UnityEditor;
using UnityEngine;

namespace Ssu_Editor.SsuDrawer {
    [CustomPropertyDrawer(typeof(OptionAttribute))]
    public class OptionDrawer : PropertyDrawer{

        private class Accessor {
            public Action<int> SetValue { get; set; }
            public Func<int, int> ValidateValue { get; set; }
            public Func<int> IndexValue { get; set; }
        }

        private Accessor _accessor;
        private string[] _items;
        private Type _variableType;

        private Stack<SerializedProperty> _stack;

        private OptionAttribute _optionAttribute => (OptionAttribute)attribute;
        private bool _show = false;

        private string[] ToArrayStrings(Array array) {
            var list = new List<string>();
            foreach (var item in array) {
                list.Add(item.ToString());
            }
            return list.ToArray();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return _show? base.GetPropertyHeight(property, label):0.0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            if (_stack == null) {
                _stack = new Stack<SerializedProperty>();
                var iterator = property.serializedObject.GetIterator();
                if (iterator.NextVisible(true)) {
                    do {
                        if (iterator.name == property.name) {
                            break;
                        }
                        _stack.Push(iterator);
                    } while (iterator.NextVisible(false));
                }   
            }

            if (_stack.Count <= 0 ) {
                return;
            }

            var previousProperty = _stack.Peek();
            if (previousProperty.stringValue == _optionAttribute.DefaultValue.ToString()) {
                _show = false;
                SetDefaultValue(property);
            } else {
                _show = true;
                Popup(position, property, label);
                EditorUtility.SetDirty(property.serializedObject.targetObject); // 重绘
            }
        }

        private void Popup(Rect position, SerializedProperty property, GUIContent label) {
            if (_accessor == null) {
                _accessor = new Accessor();
                var popupAttribute = attribute as IPopup;
                var options = popupAttribute.OptionValues as Array;
                _variableType = options.GetType().GetElementType();
                _items = ToArrayStrings(options);
                SetUp(property);
            }

            EditorGUI.BeginChangeCheck();
            var selectedIndex = EditorGUI.Popup(position, label.text, _accessor.IndexValue(), _items);
            if (EditorGUI.EndChangeCheck()) {
                _accessor.SetValue(selectedIndex);
            }

        }

        private void SetUp(SerializedProperty property) {

            if (_variableType == typeof(string)) {

                _accessor.ValidateValue = (index) => property.stringValue == _items[index] ? index : 0;
                _accessor.SetValue = (index) => property.stringValue = _items[index];
                _accessor.IndexValue = () => {
                    var index = Array.IndexOf(_items, property.stringValue);
                    return index == -1 ? 0 : index;
                };

            } else if (_variableType == typeof(int)) {

                _accessor.ValidateValue = (index) => property.intValue == Convert.ToInt32(_items[index]) ? index : 0;
                _accessor.SetValue = (index) => property.intValue = Convert.ToInt32(_items[index]);
                _accessor.IndexValue = () => {
                    var index = Array.IndexOf(_items, property.intValue);
                    return index == -1 ? 0 : index;
                };

            } else if (_variableType == typeof(double)) {

                _accessor.ValidateValue = (index) => Math.Abs(property.doubleValue - Convert.ToDouble(_items[index])) < 0.0001 ? index : 0;
                _accessor.SetValue = (index) => property.doubleValue = Convert.ToDouble(_items[index]);
                _accessor.IndexValue = () => {
                    var index = Array.IndexOf(_items, property.doubleValue);
                    return index == -1 ? 0 : index;
                };

            }

        }

        private void SetDefaultValue(SerializedProperty property) {
            var type = _optionAttribute.DefaultValue.GetType();
            if (type == typeof(string)) {
                property.stringValue = _optionAttribute.DefaultValue.ToString();
            } else if (type == typeof(int)) {
                property.intValue = Convert.ToInt32(_optionAttribute.DefaultValue);
            } else if (type == typeof(double)) {
                property.doubleValue = Convert.ToDouble(_optionAttribute.DefaultValue);
            }

        }
    }
}