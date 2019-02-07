using System;
using System.Collections.Generic;
using System.Linq;
using Ssc.SscFactory;

namespace Ssc.SscExtension {
    public static class ArrayExtension {
        public static Array MakeArray(this Type type, int length) {
            var makeme = type.MakeArrayType();
            return ObjectFactory.GetActivator<Array>(makeme.GetConstructors().First())(length);
        }

        public static void Clear<T>(this T[] array, T value) {
            if (array == null) {
                throw new ArgumentNullException(nameof(array));
            }

            for (var i = 0; i < array.Length; i++) {
                array[i] = value;
            }
        }
        
        public static void Clear<T>(this T[] array) {
            if (array == null) {
                throw new ArgumentNullException(nameof(array));
            }

            Array.Clear(array,0,array.Length);
        }
        
        
        public static int SortInsert<T>(this List<T> list, T item) {
            var search = list.BinarySearch(item);
            var index = search < 0 ? ~search : search;
            list.Insert(index, item);
            return index;
        }
    }
}