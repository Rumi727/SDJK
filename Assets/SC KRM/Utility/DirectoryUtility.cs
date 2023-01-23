using System.Collections.Generic;
using System.IO;

namespace SCKRM
{
    public static class DirectoryUtility
    {
        public static void Copy(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            string[] files = Directory.GetFiles(sourceFolder);
            string[] folders = Directory.GetDirectories(sourceFolder);

            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }

            for (int i = 0; i < folders.Length; i++)
            {
                string folder = folders[i];
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                Copy(folder, dest);
            }
        }

        public static string[] GetFiles(string path, params string[] searchPatterns) => GetFiles(path, searchPatterns, SearchOption.TopDirectoryOnly);
        [WikiIgnore]
        public static string[] GetFiles(string path, string[] searchPatterns, SearchOption searchOption)
        {
            List<string> paths = new List<string>();
            for (int i = 0; i < searchPatterns.Length; i++)
            {
                string searchPattern = searchPatterns[i];
                paths.AddRange(Directory.GetFiles(path, searchPattern, searchOption));
            }

            return paths.ToArray();
        }
    }
}
