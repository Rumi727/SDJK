using System.IO;

namespace SCKRM
{
    public static class PathUtility
    {
        public const string urlPathPrefix = "file://";

        /// <summary>
        /// (paths = ("asdf", "asdf")) = "asdf/asdf" (Path.Combine is "asdf\asdf")
        /// </summary>
        /// <param name="path1">첫번째 경로</param>
        /// <param name="path2">두번째 경로</param>
        /// <returns></returns>
        public static string Combine(string path1, string path2) => Path.Combine(path1, path2).Replace("\\", "/");
        /// <summary>
        /// (paths = ("asdf", "asdf")) = "asdf/asdf" (Path.Combine is "asdf\asdf")
        /// </summary>
        /// <param name="path1">첫번째 경로</param>
        /// <param name="path2">두번째 경로</param>
        /// <param name="path3">세번째 경로</param>
        /// <returns></returns>
        [WikiIgnore] public static string Combine(string path1, string path2, string path3) => Path.Combine(path1, path2, path3).Replace("\\", "/");
        /// <summary>
        /// (paths = ("asdf", "asdf")) = "asdf/asdf" (Path.Combine is "asdf\asdf")
        /// </summary>
        /// <param name="path1">첫번째 경로</param>
        /// <param name="path2">두번째 경로</param>
        /// <param name="path3">세번째 경로</param>
        /// <param name="path4">네번째 경로</param>
        /// <returns></returns>
        [WikiIgnore] public static string Combine(string path1, string path2, string path3, string path4) => Path.Combine(path1, path2, path3, path4).Replace("\\", "/");
        /// <summary>
        /// (paths = ("asdf", "asdf")) = "asdf/asdf" (Path.Combine is "asdf\asdf")
        /// </summary>
        /// <param name="paths">경로들</param>
        /// <returns></returns>
        [WikiIgnore] public static string Combine(params string[] paths) => Path.Combine(paths).Replace("\\", "/");

        public static string RemoveInvalidPathChars(string filename) => string.Concat(filename.Split(Path.GetInvalidPathChars()));
        public static string ReplaceInvalidPathChars(string filename) => string.Join("_", filename.Split(Path.GetInvalidPathChars()));

        public static string RemoveInvalidFileNameChars(string filename) => string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
        public static string ReplaceInvalidFileNameChars(string filename) => string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));

        public static string GetPathWithExtension(string path)
        {
            string extension = Path.GetExtension(path);
            if (extension != "")
                return path.Remove(path.Length - extension.Length);
            else
                return path;
        }

        public static string UrlPathPrefix(this string path) => urlPathPrefix + path;
    }
}
