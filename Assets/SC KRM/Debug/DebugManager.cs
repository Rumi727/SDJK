using Newtonsoft.Json;
using SCKRM.Input;
using SCKRM.SaveLoad;
using SCKRM.UI;
using SCKRM.UI.StatusBar;
using UnityEngine;

namespace SCKRM.DebugUI
{
    [WikiDescription("F3 디버그 화면을 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/Debug/UI/Debug Manager")]
    public sealed class DebugManager : UIManagerBase<DebugManager>
    {
        [GeneralSaveLoad]
        public class SaveData
        {
            static float _textRefreshDelay = 0.1f; [JsonProperty] public static float textRefreshDelay { get => _textRefreshDelay.Clamp(0); set => _textRefreshDelay = value.Clamp(0); }
            static float _graphRefreshDelay = 0.5f; [JsonProperty] public static float graphRefreshDelay { get => _graphRefreshDelay.Clamp(0); set => _graphRefreshDelay = value.Clamp(0); }



            static float _speed = 2;
            [JsonProperty] public static float graphSpeed { get => _speed.Clamp(1); set => _speed = value.Clamp(1); }



            [JsonProperty] public static bool textShow { get; set; } = true;
            [JsonProperty] public static bool graphShow { get; set; } = true;
        }



        [WikiDescription("F3 디버그 화면을 표시하는지에 대한 여부입니다")]
        public static bool isShow { get; set; } = false;



        [SerializeField] GameObject _textLayout; [WikiDescription("텍스트 레이아웃 오브젝트를 가져옵니다")] public GameObject textLayout => _textLayout;
        [SerializeField] GameObject _graphLayout; [WikiDescription("그래프 레이아웃 오브젝트를 가져옵니다")] public GameObject graphLayout => _graphLayout;



        void Update()
        {
            rectTransform.offsetMin = new Vector2(0, StatusBarManager.cropedRect.min.y);
            rectTransform.offsetMax = new Vector2(1, StatusBarManager.cropedRect.max.y);

            if (InitialLoadManager.isInitialLoadEnd && InputManager.GetKey("debug_manager.toggle", InputType.Down, InputManager.inputLockDenyAllForce))
                isShow = !isShow;

            if (textLayout.activeSelf != (isShow && SaveData.textShow))
                textLayout.SetActive(isShow && SaveData.textShow);
            if (graphLayout.activeSelf != (isShow && SaveData.graphShow))
                graphLayout.SetActive(isShow && SaveData.graphShow);
        }
    }
}
