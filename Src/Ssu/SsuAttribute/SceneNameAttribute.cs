using UnityEngine;

namespace Ssu.SsuAttribute {
    public class SceneNameAttribute : PropertyAttribute {
        public int selectedValue = 0;
        public bool enableOnly = true;
        public SceneNameAttribute(bool enableOnly = true) {
            this.enableOnly = enableOnly;
        }
    }
}
