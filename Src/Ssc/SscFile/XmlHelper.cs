using System;
using System.IO;
using System.Threading.Tasks;
using Ssc.SscExtension;
using Xml.Net;

namespace Ssc.SscFile {
    public class XmlHelper : FileHelper {
        public static async Task<T> ReadXml<T>(string fileName) where T : new() {
            if (!File.Exists(fileName)) throw new FileNotFoundException(fileName);
            var result = await Read(fileName);
            return XmlConvert.DeserializeObject<T>(result.toString(), XmlConvertOptions.ExcludeTypes);
        }

        public static async Task WriteXml<T>(string fileName, T t) {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(nameof(fileName));
            var serializeText = XmlConvert.SerializeObject(t, XmlConvertOptions.ExcludeTypes);
            await Write(fileName, serializeText.ToBytes());
        }
    }
}