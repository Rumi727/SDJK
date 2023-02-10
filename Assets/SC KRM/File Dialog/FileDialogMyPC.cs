using SCKRM.FileDialog.Drive;
using SCKRM.Object;
using SCKRM.Renderer;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SCKRM.FileDialog.MyPC
{
    [WikiDescription("파일 선택 화면의 내 PC 화면을 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/File Dialog/UI/File Dialog My PC")]
    public sealed class FileDialogMyPC : UI.UIBase
    {
        List<IObjectPooling> buttons = new List<IObjectPooling>();

        [WikiDescription("새로고침")]
        public void Refresh()
        {
            for (int i = 0; i < buttons.Count; i++)
                buttons[i].Remove();

            buttons.Clear();

            DriveInfo[] driveInfos = DriveInfo.GetDrives();
            for (int i = 0; i < driveInfos.Length; i++)
            {
                DriveInfo driveInfo = driveInfos[i];
                ButtonCreate(driveInfo.Name, driveInfo);
            }
        }

        void ButtonCreate(string path, DriveInfo driveInfo)
        {
            FileDialogMyPCButton button = (FileDialogMyPCButton)ObjectPoolingSystem.ObjectCreate("file_dialog_my_pc.button", transform).monoBehaviour;

            button.nameText.text = FileDialogDrive.GetDriveFullName(driveInfo);

            button.icon.nameSpaceIndexTypePathPair = FileDialogIcon.GetIcon(driveInfo);
            button.icon.Refresh();

            if (driveInfo.IsReady)
            {
                long availableSize = driveInfo.AvailableFreeSpace;
                long maxSize = driveInfo.TotalSize;
                float sliderValue = (float)(maxSize - availableSize) / maxSize;

                button.capacitySlider.value = sliderValue;

                if (sliderValue >= 0.9f)
                    button.capacitySliderFill.color = new Color(0.8f, 0, 0);
                else
                    button.capacitySliderFill.color = new Color(0.4f, 0.4f, 0.4f);

                button.capacityText.replace = new ReplaceOldNewPair[] { new ReplaceOldNewPair("%all%", maxSize.DataSizeToString(1)), new ReplaceOldNewPair("%value%", availableSize.DataSizeToString(1)) };
                button.capacityText.Refresh();
            }
            else
            {
                button.capacitySlider.gameObject.SetActive(false);
                button.capacityText.gameObject.SetActive(false);
            }

            buttons.Add(button);



            if (driveInfo.IsReady)
            {
                button.button.onClick.AddListener(() => onClick(path));
                static void onClick(string path) => FileDialogManager.ScreenRefresh(path);
            }
        }
    }
}
