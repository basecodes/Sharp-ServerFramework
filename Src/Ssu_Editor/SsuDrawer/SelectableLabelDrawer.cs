using Ssu.SsuAttribute;
using UnityEditor;
using UnityEngine;

namespace Ssu_Editor.SsuDrawer {
    [CustomPropertyDrawer(typeof(SelectableLabelAttribute))]
    public class SelectableLabelDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.SelectableLabel(position, selectableLabelAttribute.text);
        }

        private SelectableLabelAttribute selectableLabelAttribute => (SelectableLabelAttribute)attribute;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

            return selectableLabelAttribute.text.Split('\n').Length *
                base.GetPropertyHeight(property, label);
        }
    }
}
