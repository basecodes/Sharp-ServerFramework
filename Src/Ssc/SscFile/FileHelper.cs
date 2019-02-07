using System;
using System.IO;
using System.Threading.Tasks;

namespace Ssc.SscFile {
    public class FileHelper {
        public static async Task<byte[]> Read(string fileName) {
            if (!File.Exists(fileName)) throw new FileNotFoundException(fileName);

            using (var fileStream = new FileStream(fileName, FileMode.Open)) {
                var bytes = new byte[fileStream.Length];
                await fileStream.ReadAsync(bytes, 0, (int) fileStream.Length);
                return bytes;
            }
        }

        public static async Task Write(string fileName, byte[] bytes) {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(nameof(fileName));

//             if (!File.Exists(fileName)) {
//                 var fileInfo = new FileInfo(fileName);
//                 if (!fileInfo.Directory.Exists) throw new DirectoryNotFoundException(nameof(fileName));
//             }

            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate |
                                                             FileMode.Append)) {
                await fileStream.WriteAsync(bytes, 0, bytes.Length);
            }
        }
    }
}