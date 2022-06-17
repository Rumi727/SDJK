using SCKRM.Renderer;
using SCKRM.Resource;
using System.IO;

namespace SCKRM
{
    public static class FileDialogIcon
    {
        public static string[] textureExtension => ResourceManager.textureExtension;
        public static string[] textExtension => ResourceManager.textExtension;
        public static string[] audioExtension => ResourceManager.audioExtension;
        public static string[] videoExtension => ResourceManager.videoExtension;
        public static string[] codeExtension { get; } = new string[] { "java", "php", "scss", "cs", "css", "js", "py", "c", "cpp", "class", "fs", "go", "rb" };
        public static string[] compressedExtension { get; } = new string[] { "zip" };

        public static NameSpaceIndexTypePathPair GetIcon(string path)
        {
            if (Directory.Exists(path))
                return "sc-krm:0:gui/icon/folder";
            else if (ExtensionCheck(path, textureExtension))
                return "sc-krm:0:gui/icon/image_file";
            else if (ExtensionCheck(path, textExtension))
                return "sc-krm:0:gui/icon/text_file";
            else if (ExtensionCheck(path, audioExtension))
                return "sc-krm:0:gui/icon/music_file";
            else if (ExtensionCheck(path, videoExtension))
                return "sc-krm:0:gui/icon/video_file";
            else if (ExtensionCheck(path, codeExtension))
                return "sc-krm:0:gui/icon/code_file";
            else if (ExtensionCheck(path, compressedExtension))
                return "sc-krm:0:gui/icon/compressed_file";
            else if (ExtensionCheck(path, "exe"))
                return "sc-krm:0:gui/icon/exe_file";
            else
                return "sc-krm:0:gui/icon/file";
        }

        static bool ExtensionCheck(string path, params string[] extensions)
        {
            path = Path.GetExtension(path).ToLower();
            for (int i = 0; i < extensions.Length; i++)
            {
                if ("." + extensions[i] == path)
                    return true;
            }

            return false;
        }

        public static NameSpaceIndexTypePathPair GetIcon(DriveInfo driveInfo)
        {
            if (driveInfo.DriveType == DriveType.Fixed)
                return "sc-krm:0:gui/icon/drive";
            else if (driveInfo.DriveType == DriveType.Removable)
                return "sc-krm:0:gui/icon/usb";
            else if (driveInfo.DriveType == DriveType.CDRom)
                return "sc-krm:0:gui/icon/disc";
            else if (driveInfo.DriveType == DriveType.Network)
                return "sc-krm:0:gui/icon/network";
            else if (driveInfo.DriveType == DriveType.Ram)
                return "sc-krm:0:gui/icon/memory";
            else
                return "sc-krm:0:gui/icon/diamond_question";
        }
    }
}
