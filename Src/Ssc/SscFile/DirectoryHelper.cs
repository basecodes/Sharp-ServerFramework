using System;
using System.Collections.Generic;
using System.IO;

namespace Ssc.SscFile {
    public class DirectoryHelper {
        public static string[] GetSubdirectory(string directory) {
            if (string.IsNullOrEmpty(directory)) throw new ArgumentNullException(nameof(directory));
            var directoryInfo = new DirectoryInfo(directory);
            if (!directoryInfo.Exists) throw new DirectoryNotFoundException(nameof(directory));
            var directorys = new Stack<string>();
            while (directoryInfo.Parent != null) {
                directorys.Push(directoryInfo.Name);
                directoryInfo = directoryInfo.Parent;
            }

            directorys.Push(directoryInfo.Root.Name);
            return directorys.ToArray();
        }
    }
}