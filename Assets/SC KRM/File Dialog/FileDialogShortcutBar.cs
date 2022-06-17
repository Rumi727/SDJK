using SCKRM.Object;
using SCKRM.Renderer;
using System.Collections.Generic;
using UnityEngine;
using SCKRM.KnownFolder;
using System.IO;
using SCKRM.FileDialog.Drive;

namespace SCKRM.FileDialog.ShortcurBar
{
    [AddComponentMenu("SC KRM/File Dialog/UI/File Dialog Shortcur Bar")]
    public sealed class FileDialogShortcutBar : UI.UI
    {
        List<IObjectPooling> buttons = new List<IObjectPooling>();

        public void Refresh()
        {
            for (int i = 0; i < buttons.Count; i++)
                buttons[i].Remove();

            buttons.Clear();

            ShortcutBarCreate("sc-krm:0:gui/icon/computer", "내 PC", "", true);

            LineCreate();

            DriveInfo[] driveInfos = DriveInfo.GetDrives();
            for (int i = 0; i < driveInfos.Length; i++)
            {
                DriveInfo driveInfo = driveInfos[i];
                ShortcutBarCreate(FileDialogIcon.GetIcon(driveInfo), new NameSpacePathReplacePair(FileDialogDrive.GetDriveFullName(driveInfo)), driveInfo.RootDirectory.ToString(), driveInfo.IsReady);
            }

#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
            LineCreate();
            
            ShortcutBarCreate("sc-krm:0:gui/icon/3d", "sc-krm:file_dialog.3d_objects", KnownFolders.GetPath(KnownFolderType.Objects3D), true);
            ShortcutBarCreate("sc-krm:0:gui/icon/download", "sc-krm:file_dialog.downloads", KnownFolders.GetPath(KnownFolderType.Downloads), true);
            ShortcutBarCreate("sc-krm:0:gui/icon/video", "sc-krm:file_dialog.videos", KnownFolders.GetPath(KnownFolderType.Videos), true);
            ShortcutBarCreate("sc-krm:0:gui/icon/text_file", "sc-krm:file_dialog.documents", KnownFolders.GetPath(KnownFolderType.Documents), true);
            ShortcutBarCreate("sc-krm:0:gui/icon/desktop", "sc-krm:file_dialog.desktop", KnownFolders.GetPath(KnownFolderType.Desktop), true);
            ShortcutBarCreate("sc-krm:0:gui/icon/picture", "sc-krm:file_dialog.pictures", KnownFolders.GetPath(KnownFolderType.Pictures), true);
            ShortcutBarCreate("sc-krm:0:gui/icon/music", "sc-krm:file_dialog.music", KnownFolders.GetPath(KnownFolderType.Music), true);
#endif
        }

        void ShortcutBarCreate(NameSpaceIndexTypePathPair icon, NameSpacePathReplacePair text, string path, bool isReady)
        {
            FileDialogShortcutBarButton button = (FileDialogShortcutBarButton)ObjectPoolingSystem.ObjectCreate("file_dialog_shortcut_bar.button", transform).monoBehaviour;

            button.icon.nameSpaceIndexTypePathPair = icon;
            button.text.nameSpacePathReplacePair = text;

            button.icon.Refresh();
            button.text.Refresh();

            buttons.Add(button);


            if (isReady)
            {
                button.button.onClick.AddListener(() => onClick(path));
                static void onClick(string path) => FileDialogManager.ScreenRefresh(path);
            }
        }

        void LineCreate() => buttons.Add(ObjectPoolingSystem.ObjectCreate("file_dialog_shortcut_bar.line", transform, false).objectPooling);
    }
}
