using UnityEngine;

namespace Ssu.SsuAttribute {
    public class SelectableLabelAttribute : PropertyAttribute {
        public string text;

        public SelectableLabelAttribute(string text) {
            this.text = text;
        }
    }
}
