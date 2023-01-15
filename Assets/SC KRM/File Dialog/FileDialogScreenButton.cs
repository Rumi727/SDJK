using PolyAndCode.UI;
using SCKRM.Renderer;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SCKRM.FileDialog.Screen
{
    [WikiDescription("파일 선택 화면의 버튼을 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/File Dialog/UI/File Dialog Screen Button")]
    public sealed class FileDialogScreenButton : UI.UI, ICell, IPointerClickHandler
    {
        [SerializeField, NotNull] Toggle _toggle; public Toggle toggle { get => _toggle; }

        [SerializeField, NotNull] CustomSpriteRendererBase _icon; public CustomSpriteRendererBase icon { get => _icon; }
        [SerializeField, NotNull] TMP_Text _text; public TMP_Text text { get => _text; }




        static string lastSelectedFilePath = "";

        string path = "";
        string[] allPaths = new string[0];
        string fileName = "";

        bool isFolder = false;

        bool doubleClick = false;
        float doubleClickTimer = 0;
        void Update()
        {
            if (doubleClick && doubleClickTimer <= 0.5f)
                doubleClickTimer += Kernel.unscaledDeltaTime;
            else
            {
                doubleClick = false;
                doubleClickTimer = 0;
            }

            toggle.isOn = FileDialogManager.selectedFilePath.Contains(path);
        }

        [WikiDescription("버튼 설정")]
        public void ConfigureCell(string path, ToggleGroup toggleGroup, string[] allPaths)
        {
            this.path = path;
            this.allPaths = allPaths;
            fileName = Path.GetFileName(path);

            isFolder = Directory.Exists(path);

            doubleClick = false;
            doubleClickTimer = 0;

            toggle.group = toggleGroup;

            text.text = fileName;

            icon.nameSpaceIndexTypePathPair = FileDialogIcon.GetIcon(path);
            icon.Refresh();
        }

        [WikiDescription("클릭했을 때")]
        public void OnPointerClick(PointerEventData eventData)
        {
            if (doubleClick)
            {
                //더블 클릭인데 폴더일경우 그 폴더로 들어갑니다
                if (isFolder)
                    FileDialogManager.ScreenRefresh(path);
                else //더블 클릭인데 파일이면 그 파일을 선택하고 선택 창을 종료합니다
                {
                    FileDialogManager.selectedFilePath.Clear();
                    FileDialogManager.selectedFilePath.Add(path);

                    FileDialogManager.HideForce(true);
                }
            }
            else
            {
                //더블클릭이 아닐때 폴더가 아니거나 폴더 선택 모드로 열려있으면 FileOnClick 메소드를 호출합니다
                if (!isFolder || FileDialogManager.isFolderOpenMode)
                    FileSelect();
            }

            doubleClick = true;

            void FileSelect()
            {
                if (toggle.group == null)
                {
                    if (!UnityEngine.Input.GetKey(KeyCode.LeftControl))
                        FileDialogManager.selectedFilePath.Clear();

                    if (!UnityEngine.Input.GetKey(KeyCode.LeftShift))
                    {
                        if (toggle.isOn)
                            FileDialogManager.selectedFilePath.Add(path);
                        else
                            FileDialogManager.selectedFilePath.Remove(path);
                    }
                    else
                    {
                        (int startIndex, int endIndex) index = (Array.IndexOf(allPaths, lastSelectedFilePath), Array.IndexOf(allPaths, path));
                        if (index.endIndex < index.startIndex)
                            index = (index.endIndex, index.startIndex);

                        for (int i = index.startIndex; i <= index.endIndex; i++)
                        {
                            string path = allPaths[i];
                            if (!FileDialogManager.selectedFilePath.Contains(path))
                                FileDialogManager.selectedFilePath.Add(path);
                        }
                    }
                }
                else
                {
                    if (toggle.isOn)
                    {
                        FileDialogManager.selectedFilePath.Clear();
                        FileDialogManager.selectedFilePath.Add(path);

                        FileDialogManager.SetFileName(fileName);
                    }
                    else
                    {
                        FileDialogManager.selectedFilePath.Remove(path);
                        FileDialogManager.SetFileName("");
                    }
                }

                lastSelectedFilePath = path;
            }
        }
    }
}
