using Newtonsoft.Json;
using SCKRM.Input;
using SCKRM.Json;
using SCKRM.SaveLoad;
using SCKRM.UI.StatusBar;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SCKRM.UI
{
    [AddComponentMenu("SC KRM/UI/UI Manager")]
    public sealed class UIManager : Manager<UIManager>
    {
        [GeneralSaveLoad]
        public sealed class SaveData
        {
            [JsonProperty] public static JColor systemColor { get; set; } = new JColor32(131, 26, 160);

            static float _guiSize = 1; [JsonProperty] public static float guiSize { get => _guiSize.Clamp(0.5f, 4); set => _guiSize = value.Clamp(0.5f, 4); }
            static float _fixedGuiSize = 1; [JsonProperty] public static float fixedGuiSize
            {
                get => _fixedGuiSize.Clamp(scaleAccordingToScreenSize * 0.5f, scaleAccordingToScreenSize * 4f);
                set => _fixedGuiSize = value.Clamp(scaleAccordingToScreenSize * 0.5f, scaleAccordingToScreenSize * 4f);
            }
            static bool _fixedGuiSizeEnable = true; [JsonProperty] public static bool fixedGuiSizeEnable { get => _fixedGuiSizeEnable; set => _fixedGuiSizeEnable = value; }
        }

        public static float currentGuiSize { get; private set; } = 1;
        public static float scaleAccordingToScreenSize { get; private set; } = 1;

        [SerializeField] Canvas _kernelCanvas; public Canvas kernelCanvas => _kernelCanvas;
        [SerializeField] RectTransform _kernelSideBarRectTransform; public RectTransform kernelCanvasUI => _kernelSideBarRectTransform;
        [SerializeField] TMP_Text _exceptionText; public TMP_Text exceptionText => _exceptionText;



        static List<Action> backEventList { get; } = new List<Action>();
        static List<Action> highPriorityBackEventList { get; } = new List<Action>();

        public static event Action homeEvent = delegate { };



        void Awake() => SingletonCheck(this);

        void Update()
        {
            //현제 해상도의 가로랑 1920을 나눠서 모든 해상도에도 가로 픽셀 크기는 똑같이 유지되게 함
            scaleAccordingToScreenSize = ScreenManager.width / 1920f;

            //GUI 크기 설정
            //고정 GUI 크기가 꺼져있다면 화면 크기에 따라 유동적으로 GUI 크기가 변경됩니다
            if (!SaveData.fixedGuiSizeEnable)
                currentGuiSize = SaveData.guiSize * scaleAccordingToScreenSize;
            else //고정 GUI 크기가 켜져있다면 GUI 크기를 고정시킵니다
                currentGuiSize = SaveData.fixedGuiSize;

            if (InitialLoadManager.isInitialLoadEnd)
            {
                if (InputManager.GetKey("gui.back", InputType.Down, InputManager.inputLockDenyAllForce))
                    BackEventInvoke();
                else if (InputManager.GetKey("gui.home", InputType.Down, InputManager.inputLockDenyAll))
                    homeEvent.Invoke();
            }



            kernelCanvasUI.offsetMin = StatusBarManager.cropedRect.min;
            kernelCanvasUI.offsetMax = StatusBarManager.cropedRect.max;
        }

        public static void BackEventInvoke()
        {
            if (highPriorityBackEventList.Count > 0)
                highPriorityBackEventList[0].Invoke();
            else if (backEventList.Count > 0)
                backEventList[0].Invoke();
        }

        public static void HomeEventInvoke() => homeEvent.Invoke();

        public static void BackEventAdd(Action action, bool highPriority = false)
        {
            if (highPriority)
                highPriorityBackEventList.Insert(0, action);
            else
                backEventList.Insert(0, action);
        }

        public static void BackEventRemove(Action action, bool highPriority = false)
        {
            if (highPriority)
                highPriorityBackEventList.Remove(action);
            else
                backEventList.Remove(action);
        }
    }
}