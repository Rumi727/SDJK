using PolyAndCode.UI;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.FileDialog.Screen
{
    [WikiDescription("파일 선택 화면을 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/File Dialog/UI/File Dialog Screen")]
    public sealed class FileDialogScreen : UI.UI, IRecyclableScrollRectDataSource
    {
        [SerializeField] ToggleGroup toggleGroup;
        [SerializeField] Transform content;
        [SerializeField] RecyclableScrollRect recyclableScrollRect;

        string[] directorys = new string[0];
        string[] files = new string[0];

        protected override void Awake() => recyclableScrollRect.Initialize(this);

        [WikiDescription("새로고침")]
        public void Refresh()
        {
            string path = FileDialogManager.currentPath;
            if (string.IsNullOrEmpty(path))
                return;

            try
            {
                directorys = Directory.GetDirectories(path);

                if (!FileDialogManager.isFolderOpenMode)
                    files = DirectoryUtility.GetFiles(path, FileDialogManager.currentFilter.ToSearchPatterns());
                else
                    files = new string[0];

                if (!string.IsNullOrEmpty(FileDialogManager.currentSearch))
                {
                    directorys = Array.FindAll(directorys, (string value) => value.ToLower().Contains(FileDialogManager.currentSearch.ToLower()));
                    files = Array.FindAll(files, (string value) => value.ToLower().Contains(FileDialogManager.currentSearch.ToLower()));
                }
            }
            catch (UnauthorizedAccessException)
            {
                FileDialogManager.Up();
                return;
            }
            catch (PathTooLongException)
            {
                FileDialogManager.Up();
                return;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                FileDialogManager.Up();
                return;
            }

            recyclableScrollRect.ReloadData();
        }

        [WikiDescription("표시된 아이템이 개수")]
        public int GetItemCount() => directorys.Length + files.Length;

        [WikiDescription("파일 선택 화면의 버튼을 설정")]
        public void SetCell(ICell cell, int index)
        {
            FileDialogScreenButton item = (FileDialogScreenButton)cell;
            ToggleGroup toggleGroup;
            string[] allPaths;

            if (FileDialogManager.isFolderOpenMode)
                allPaths = directorys;
            else
                allPaths = files;

            if (FileDialogManager.isSingle)
                toggleGroup = this.toggleGroup;
            else
                toggleGroup = null;

            if (index < directorys.Length)
                item.ConfigureCell(directorys[index], toggleGroup, allPaths);
            else if (index - directorys.Length < files.Length)
                item.ConfigureCell(files[index - directorys.Length], toggleGroup, allPaths);
        }
    }
}
