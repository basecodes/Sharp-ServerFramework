using System;
using System.Collections.Generic;
using Ssu.SsuAttribute;
using UnityEditor;
using UnityEngine;

namespace Ssu_Editor.SsuDrawer {

    [CustomPropertyDrawer(typeof(PopupAttribute))]
    public class PopupDrawer : PropertyDrawer {

        private class Accessor {
            public Action<int> SetValue { get; set; }
            public Func<int, int> ValidateValue { get; set; }
            public Func<int> IndexValue { get; set; }
        }

        private Accessor _accessor;
        private string[] _items;
        private Type _variableType;

        private string[] ToArrayStrings(Array array) {
            var list = new List<string>();
            foreach (var item in array) {
                list.Add(item.ToString());
            }
            return list.ToArray();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
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
                _accessor.SetValue = (index) =>  property.stringValue = _items[index];
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
                
                _accessor.ValidateValue = (index) => property.doubleValue == Convert.ToDouble(_items[index]) ? index : 0;
                _accessor.SetValue = (index) => property.doubleValue = Convert.ToDouble(_items[index]);
                _accessor.IndexValue = () => {
                    var index = Array.IndexOf(_items, property.doubleValue);
                    return index == -1 ? 0 : index;
                };

            }

        }
    }
}
