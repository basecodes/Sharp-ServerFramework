using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ssc.SscExtension;
using Ssc.SscFactory;

namespace Ssc.SscFile {
    public class CSVHelper : FileHelper {
        public static async Task<T[]> FromCSV<T>(
            string fileName,
            string colsSplitFlag = ",",
            string cellsSplitFlag = ";",
            string rowsSplitFlag = "\r\n") {
            if (!File.Exists(fileName)) throw new FileNotFoundException(nameof(fileName));

            var strings = (await Read(fileName)).toString();

            var rowsSplitChars = rowsSplitFlag.ToArray();
            var lines = strings.Split(rowsSplitChars, StringSplitOptions.RemoveEmptyEntries);

            var colsSplitChars = colsSplitFlag.ToArray();
            var headerLine = lines.First().Split(colsSplitChars);

            var ts = new T[lines.Length - 1];

            var properys = typeof(T).GetProperties();

            var cellsSplitChars = cellsSplitFlag.ToArray();
            for (var i = 1; i < lines.Length; ++i) {
                var v = ObjectFactory.GetActivator<T>(typeof(T).GetConstructors().First())();
                ts[i - 1] = v;

                var cols = lines[i].Split(colsSplitChars);
                for (var j = 0; j < cols.Length; j++) {
                    var propery = properys[j];
                    var type = propery.PropertyType;

                    if (type.IsGenericType) {
                        var valueType = type.GetGenericArguments()[0];
                        var listType = typeof(List<>);
                        var makeme = listType.MakeGenericType(valueType);
                        var obj = ObjectFactory.GetActivator<object>(makeme.GetConstructors().First())() as IList;
                        var cells = cols[j].Split(cellsSplitChars);
                        for (var y = 0; y < cells.Length; y++) obj.Add(Convert.ChangeType(cells[y], valueType));
                        propery.SetValue(v, obj);
                    } else {
                        propery.SetValue(v, Convert.ChangeType(cols[j], type));
                    }
                }
            }

            return ts;
        }
    }
}