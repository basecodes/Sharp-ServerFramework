using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ssc.SscFile {
    public class JsonHelper : FileHelper {

        public static string ToJson<T>(T obj) {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return json;
        }

        public static T FromJson<T>(string jsonContent) {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (var streamReader = new StringReader(jsonContent)) {
                using (var reader = new JsonTextReader(streamReader)) {
                    return serializer.Deserialize<T>(reader);
                }
            }
        }

        public static T FromJsonFile<T>(string fileName) {
            if (!File.Exists(fileName)) {
                throw new FileNotFoundException(fileName);
            }

            var serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (var fileStream = new FileStream(fileName, FileMode.Open)) {
                using (var streamReader = new StreamReader(fileStream)) {
                    using (var reader = new JsonTextReader(streamReader)) {
                        return serializer.Deserialize<T>(reader);
                    }
                }
            }
        }

        public static void FromJsonFile<T>(string fileName,T obj) {
            if (!File.Exists(fileName)) {
                throw new FileNotFoundException(fileName);
            }

            var serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (var fileStream = new FileStream(fileName, FileMode.Open)) {
                using (var streamReader = new StreamReader(fileStream)) {
                    using (var reader = new JsonTextReader(streamReader)) {
                        serializer.Populate(reader,obj);
                    }
                }
            }
        }

        public static void ToJson<T>(string fileName, T obj) {
            if (string.IsNullOrEmpty(fileName)) {
                throw new ArgumentNullException(nameof(fileName));
            }
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate)) {
                using (var streamWriter = new StreamWriter(fileStream)) {
                    using (var writer = new JsonTextWriter(streamWriter)) {
                        serializer.Serialize(writer, obj);
                    }
                }
            }
        }
    }
}