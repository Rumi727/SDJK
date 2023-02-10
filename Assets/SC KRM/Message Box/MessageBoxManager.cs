using Cysharp.Threading.Tasks;
using SCKRM.Input;
using SCKRM.Object;
using SCKRM.Renderer;
using SCKRM.Threads;
using SCKRM.UI.StatusBar;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SCKRM.UI.Overlay.MessageBox
{
    [WikiDescription("메시지 박스를 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/Message Box/UI/Message Box Manager")]
    public sealed class MessageBoxManager : UIManagerBase<MessageBoxManager>, IUIOverlay
    {
        [SerializeField] CanvasGroup messageBoxCanvasGroup;
        [SerializeField] GameObject messabeBoxBG;
        [SerializeField] Transform messageBoxButtons;
        [SerializeField] CustomTextRendererBase messageBoxInfo;
        [SerializeField] CustomSpriteRendererBase messageBoxIcon;

        static MessageBoxButton[] createdMessageBoxButton = new MessageBoxButton[0];
        public static bool isMessageBoxShow { get; private set; } = false;



        protected override void Awake() => SingletonCheck(this);

        void Update()
        {
            if (isMessageBoxShow)
            {
                messageBoxCanvasGroup.alpha = messageBoxCanvasGroup.alpha.MoveTowards(1, 0.15f * Kernel.fpsUnscaledSmoothDeltaTime);

                messageBoxCanvasGroup.interactable = true;
                messageBoxCanvasGroup.blocksRaycasts = true;

                if (!messabeBoxBG.activeSelf)
                    messabeBoxBG.SetActive(true);
            }
            else
            {
                messageBoxCanvasGroup.alpha = messageBoxCanvasGroup.alpha.MoveTowards(0, 0.15f * Kernel.fpsUnscaledSmoothDeltaTime);

                if (messageBoxCanvasGroup.alpha <= 0)
                {
                    if (messabeBoxBG.activeSelf)
                        messabeBoxBG.SetActive(false);
                }

                messageBoxCanvasGroup.interactable = false;
                messageBoxCanvasGroup.blocksRaycasts = false;
            }
        }



        /// <summary>
        /// 메시지 박스를 활성화 합니다
        /// </summary>
        /// <param name="button">
        /// 표시할 버튼
        /// </param>
        /// <param name="defaultIndex">
        /// 기본으로 선택 할 인덱스
        /// </param>
        /// <param name="info">
        /// 설명
        /// </param>
        /// <param name="icon">
        /// 아이콘
        /// </param>
        /// <returns>
        /// 선택한 인덱스
        /// </returns>
        [WikiDescription("메시지 박스를 활성화 합니다")]
        public static async UniTask<int> Show(NameSpacePathReplacePair button, int defaultIndex, NameSpacePathReplacePair info, NameSpaceIndexTypePathPair icon) => await show(new NameSpacePathReplacePair[] { button }, defaultIndex, info, icon);
        /// <summary>
        /// 메시지 박스를 활성화 합니다
        /// </summary>
        /// <param name="button">
        /// 표시할 버튼
        /// </param>
        /// <param name="defaultIndex">
        /// 기본으로 선택 할 인덱스
        /// </param>
        /// <param name="info">
        /// 설명
        /// </param>
        /// <param name="icon">
        /// 아이콘
        /// </param>
        /// <returns>
        /// 선택한 인덱스
        /// </returns>
        [WikiIgnore] public static async UniTask<int> Show(NameSpacePathReplacePair[] buttons, int defaultIndex, NameSpacePathReplacePair info, NameSpaceIndexTypePathPair icon) => await show(buttons, defaultIndex, info, icon);

        static async UniTask<int> show(NameSpacePathReplacePair[] buttons, int defaultIndex, NameSpacePathReplacePair info, NameSpaceIndexTypePathPair icon)
        {
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException();
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException();
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException();

            if (isMessageBoxShow)
                return defaultIndex;
            else if (defaultIndex < 0 || buttons.Length < defaultIndex)
                return defaultIndex;

            await UniTask.WaitUntil(() => instance != null);

            isMessageBoxShow = true;
            UIOverlayManager.showedOverlays.Add(instance);

            instance.messageBoxIcon.nameSpaceIndexTypePathPair = icon;
            instance.messageBoxIcon.Refresh();

            instance.messageBoxInfo.nameSpacePathReplacePair = info;
            instance.messageBoxInfo.Refresh();

            #region Button Object Create
            for (int i = 0; i < createdMessageBoxButton.Length; i++)
                createdMessageBoxButton[i].Remove();

            createdMessageBoxButton = new MessageBoxButton[buttons.Length];
            for (int i = 0; i < buttons.Length; i++)
            {
                MessageBoxButton button = (MessageBoxButton)ObjectPoolingSystem.ObjectCreate("window_manager.message_box_button", instance.messageBoxButtons).monoBehaviour;
                createdMessageBoxButton[i] = button;

                button.index = i;

                button.text.nameSpacePathReplacePair = buttons[i];
                button.text.Refresh();

                //버튼이 눌렸을때, UI를 닫기 위해서 버튼에 있는 OnClick 이벤트에 clickedIndex를 button.index로 바꾸는 action 메소드를 추가합니다
                button.button.onClick.AddListener(() => action(button));
            }

            for (int i = 0; i < createdMessageBoxButton.Length; i++)
            {
                Navigation navigation = new Navigation();
                navigation.mode = Navigation.Mode.Explicit;

                if (i > 0)
                    navigation.selectOnUp = createdMessageBoxButton[i - 1].button;
                if (i < createdMessageBoxButton.Length - 1)
                    navigation.selectOnDown = createdMessageBoxButton[i + 1].button;

                createdMessageBoxButton[i].button.navigation = navigation;
            }
            #endregion

            GameObject previousTabSelectGameObject = StatusBarManager.tabSelectGameObject;
            StatusBarManager.tabSelectGameObject = createdMessageBoxButton[defaultIndex].gameObject;

            GameObject previouslySelectedGameObject = EventSystem.current.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(createdMessageBoxButton[defaultIndex].gameObject);

            bool previouslyForceInputLock = InputManager.forceInputLock;
            InputManager.forceInputLock = true;



            int clickedIndex = -1;

            //Back 버튼을 눌렀을때, UI를 닫기 위해 이벤트에 clickedIndex를 defaultIndex로 변경하는 BackEvent 메소드를 추가합니다
            UIManager.BackEventAdd(backEvent, true);

            await UniTask.WaitUntil(() => clickedIndex >= 0, PlayerLoopTiming.Initialization, instance.GetCancellationTokenOnDestroy());

            return select(clickedIndex);



            int select(int index)
            {
                isMessageBoxShow = false;
                UIOverlayManager.showedOverlays.Remove(instance);

                StatusBarManager.tabSelectGameObject = previousTabSelectGameObject;
                EventSystem.current.SetSelectedGameObject(previouslySelectedGameObject);

                InputManager.forceInputLock = previouslyForceInputLock;

                UIManager.BackEventRemove(backEvent, true);

                return index;
            }

            void backEvent() => clickedIndex = defaultIndex;
            void action(MessageBoxButton messageBoxButton) => clickedIndex = messageBoxButton.index;
        }
    }
}
