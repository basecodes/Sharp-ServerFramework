using UnityEngine;

namespace Ssu.SsuAttribute {

    public interface IPopup {
        object OptionValues { get; }
    }

    public class PopupAttribute : PropertyAttribute,IPopup {

        public object OptionValues { get; }

        public PopupAttribute(params int[] optionValues) {
            OptionValues = optionValues;
        }

        public PopupAttribute(params double[] optionValues) {
            OptionValues = optionValues;
        }

        public PopupAttribute(params string[] optionValues) {
            OptionValues = optionValues;
        }
    }
}
