using UnityEngine;

namespace Ssu.SsuAttribute {
    public class OptionAttribute:PropertyAttribute,IPopup {
        public object DefaultValue { get; }
        public object OptionValues { get; }

        public OptionAttribute(string defaultValue = "",params string[] optionValues) {
            DefaultValue = defaultValue;
            OptionValues = optionValues;
        }

        public OptionAttribute(double defaultValue = 0.0d,params double[] optionValues) {
            DefaultValue = defaultValue;
            OptionValues = optionValues;
        }

        public OptionAttribute(int defaultValue = 0,params int[] optionValues) {
            DefaultValue = defaultValue;
            OptionValues = optionValues;
        }
    }
}