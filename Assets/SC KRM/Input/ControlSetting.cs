using Cysharp.Threading.Tasks;
using SCKRM.UI;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SCKRM.Input.UI
{
    [WikiDescription("조작 설정을 관리하는 클래스 입니다")]
    public sealed class ControlSetting : UIBase
    {
        readonly static List<KeyCode> emptyKeyCodeList = new List<KeyCode>();



        [SerializeField] string _targetKey;
        public string targetKey => _targetKey;

        public bool isSelected { get; private set; }



        [SerializeField, NotNull] RectTransform controlPanelRectTransform;
        [SerializeField, NotNull] CanvasGroup resetButton;
        [SerializeField, NotNull] RectTransform nameRectTransform;
        [SerializeField, NotNull] Image controlButtonImage;
        [SerializeField, NotNull] TMP_Text controlButtonText;

        protected override async void Awake()
        {
            if (await UniTask.WaitUntil(() => InitialLoadManager.isSettingLoadEnd, PlayerLoopTiming.Update, AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                return;

            TextRefresh();
        }

        protected override void OnEnable() => InputManager.controlSaveDataResetEvent += TextRefresh;
        protected override void OnDisable() => InputManager.controlSaveDataResetEvent -= TextRefresh;

        void Update()
        {
            if (!InitialLoadManager.isSettingLoadEnd)
                return;

            if (isSelected)
                controlButtonImage.color = controlButtonImage.color.Lerp(UIManager.SaveData.systemColor, 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
            else
                controlButtonImage.color = controlButtonImage.color.Lerp(new Color(0.05098039f, 0.05098039f, 0.05098039f), 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);

            nameRectTransform.offsetMax = new Vector2(-(controlButtonImage.rectTransform.rect.size.x + 96), nameRectTransform.offsetMax.y);

            if (InputManager.SaveData.controlSettingList.ContainsKey(targetKey))
            {
                resetButton.interactable = true;
                controlPanelRectTransform.offsetMin = controlPanelRectTransform.offsetMin.Lerp(new Vector2(35, controlPanelRectTransform.offsetMin.y), 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
                resetButton.alpha = resetButton.alpha.Lerp(1, 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);

                if (resetButton.alpha > 0.99f)
                    resetButton.alpha = 1;
            }
            else
            {
                resetButton.interactable = false;
                controlPanelRectTransform.offsetMin = controlPanelRectTransform.offsetMin.Lerp(new Vector2(0, controlPanelRectTransform.offsetMin.y), 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
                resetButton.alpha = resetButton.alpha.Lerp(0, 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);

                if (resetButton.alpha < 0.01f)
                    resetButton.alpha = 0;
            }
        }



        public void TextRefresh()
        {
            if (InputManager.controlSettingList.TryGetValue(targetKey, out List<KeyCode> keyCodes))
                TextRefresh(keyCodes);
        }

        public void TextRefresh(List<KeyCode> keyCodes)
        {
            controlButtonText.text = "";
            for (int i = 0; i < keyCodes.Count; i++)
            {
                if (i >= keyCodes.Count - 1)
                    controlButtonText.text += keyCodes[i].KeyCodeToString();
                else
                    controlButtonText.text += keyCodes[i].KeyCodeToString() + " + ";
            }
        }



        public async void ControlChange()
        {
            if (isSelected || !InputManager.controlSettingList.ContainsKey(targetKey))
                return;
            
            InputManager.forceInputLock = true;
            isSelected = true;
            UIManager.BackEventAdd(Cancel, true);

            EventSystem.current.SetSelectedGameObject(null);

            CancellationToken cancelToken = gameObject.GetCancellationTokenOnDestroy();

            if (await UniTask.WaitUntil(() => !InputManager.GetAnyKey("all", "force"), PlayerLoopTiming.Update, cancelToken).SuppressCancellationThrow())
                return;

            List<KeyCode> keyDowns = new List<KeyCode>();
            bool loopBreak = false;
            bool cancel = false;
            while (!loopBreak)
            {
                if (!gameObject.activeInHierarchy)
                    break;

                for (int i = 0; i < InputManager.unityKeyCodeList.Length; i++)
                {
                    bool keyCancel = false;
                    KeyCode key = InputManager.unityKeyCodeList[i];
                    if (key == KeyCode.Escape || key == KeyCode.Return || key == KeyCode.KeypadEnter)
                        continue;
                    else if (key == KeyCode.Print || key == KeyCode.SysReq || key == KeyCode.LeftWindows || key == KeyCode.RightWindows || key == KeyCode.LeftCommand || key == KeyCode.RightCommand)
                        keyCancel = true;

                    if (InputManager.GetKey(key, InputType.Down, "all", "force"))
                    {
                        if (keyCancel)
                            Cancel();
                        else
                        {
                            keyDowns.Add(key);
                            TextRefresh(keyDowns);
                        }
                    }
                    else if (InputManager.GetKey(key, InputType.Up, "all", "force"))
                    {
                        loopBreak = true;
                        break;
                    }
                }

                if (await UniTask.DelayFrame(1, PlayerLoopTiming.Update, cancelToken).SuppressCancellationThrow())
                    return;
            }

            if (!cancel)
            {
                if (InputManager.SaveData.controlSettingList.ContainsKey(targetKey))
                    InputManager.SaveData.controlSettingList[targetKey] = keyDowns;
                else
                    InputManager.SaveData.controlSettingList.Add(targetKey, keyDowns);
            }

            InputManager.ControlListRefresh();
            TextRefresh();

            InputManager.forceInputLock = false;
            isSelected = false;
            UIManager.BackEventRemove(Cancel, true);

            void Cancel()
            {
                loopBreak = true;
                cancel = true;
            }
        }

        public void ResetButton()
        {
            InputManager.SaveData.controlSettingList.Remove(targetKey);
            InputManager.ControlListRefresh();
            TextRefresh();
        }

        public void Emptying()
        {
            if (InputManager.SaveData.controlSettingList.ContainsKey(targetKey))
                InputManager.SaveData.controlSettingList[targetKey] = emptyKeyCodeList;
            else
                InputManager.SaveData.controlSettingList.Add(targetKey, emptyKeyCodeList);

            InputManager.ControlListRefresh();
            TextRefresh();
        }
    }
}
