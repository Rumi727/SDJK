using SCKRM.UI;
using SCKRM.UI.StatusBar;
using System;
using UnityEngine;

namespace SCKRM.Camera
{
    [WikiDescription("모든 카메라에 기본으로 붙는 카메라 유틸 클래스 입니다")]
    [ExecuteAlways]
    [AddComponentMenu("SC KRM/Camera/Camera Setting")]
    public sealed class CameraSetting : MonoBehaviour
    {
#pragma warning disable CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.
        [WikiDescription("붙어있는 카메라 컴포넌트를 가져옵니다")]
        public UnityEngine.Camera camera => _camera = this.GetComponentFieldSave(_camera, ComponentTool.GetComponentMode.destroyIfNull); [NonSerialized] UnityEngine.Camera _camera;
#pragma warning restore CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.

        /// <summary>
        /// 스크립트가 카메라의 설정을 변경하지 못하게 막습니다
        /// </summary>
        [WikiDescription("스크립트가 카메라의 설정을 변경하지 못하게 막습니다")]
        public bool customSetting { get => _customSetting; set => _customSetting = value; } [SerializeField] bool _customSetting;

        void Update()
        {
            if (Kernel.isPlaying)
            {
                if (camera == null)
                    return;
                else if (customSetting)
                    return;

                RectTransform taskBar = StatusBarManager.instance.rectTransform;
                if (StatusBarManager.cropTheScreen)
                {
                    if (!StatusBarManager.SaveData.bottomMode)
                        camera.rect = new Rect(0, 0, 1, 1 - ((taskBar.rect.size.y - taskBar.anchoredPosition.y) * UIManager.currentGuiSize / ScreenManager.height));
                    else
                    {
                        float y = (taskBar.rect.size.y + taskBar.anchoredPosition.y) * UIManager.currentGuiSize / ScreenManager.height;
                        camera.rect = new Rect(0, y, 1, 1 - y);
                    }
                }
                else
                    camera.rect = new Rect(0, 0, 1, 1);
            }
        }
    }
}