using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ssc.SscFactory;

namespace Ssc.SscExtension {
    public static class DictionaryExtension {
        public static IDictionary MakeDictionary(Type keyType, Type valueType) {
            var dictType = typeof(Dictionary<,>);
            var makeme = dictType.MakeGenericType(keyType, valueType);
            return ObjectFactory.GetActivator<IDictionary>(makeme.GetConstructors().First())();
        }
    }
}