using UnityEngine;

namespace Ssu.SsuAttribute {
    public class ObserveAttribute : PropertyAttribute {
        public string[] callbackNames;

        public ObserveAttribute(params string[] callbackNames) {
            this.callbackNames = callbackNames;
        }
    }
}
