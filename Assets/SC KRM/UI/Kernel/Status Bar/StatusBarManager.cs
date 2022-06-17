using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SCKRM.Input;
using SCKRM.SaveLoad;
using SCKRM.UI.SideBar;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SCKRM.UI.StatusBar
{
    [AddComponentMenu("SC KRM/UI/Kerenl/Status Bar/Status Bar Manager")]
    [RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Image))]
    public sealed class StatusBarManager : UIManager<StatusBarManager>, IPointerEnterHandler, IPointerExitHandler
    {
        [GeneralSaveLoad]
        public sealed class SaveData
        {
            [JsonProperty] public static bool bottomMode { get; set; } = false;
            [JsonProperty] public static bool twentyFourHourSystem { get; set; } = false;
            [JsonProperty] public static bool toggleSeconds { get; set; } = false;
        }

        public static bool allowStatusBarShow { get; set; } = false;
        public static bool backButtonShow { get; set; } = true;

        public static bool selectedStatusBar { get; private set; } = false;
        public static bool isStatusBarShow { get; private set; } = false;


        static bool _cropTheScreen = true; public static bool cropTheScreen
        {
            get => _cropTheScreen;
            set
            {
                if (allowStatusBarShow)
                    return;

                _cropTheScreen = value;
            }
        }
        public static Rect cropedRect { get; private set; } = Rect.zero;

        public static GameObject tabSelectGameObject { get; set; } = null;
        public static bool tabAllow { get; set; } = false;



        [SerializeField, HideInInspector] Image _image; public Image image => _image = this.GetComponentFieldSave(_image);

        [SerializeField] Image _background; public Image background => _background;
        [SerializeField] GameObject _backButton; public GameObject backButton => _backButton;
        [SerializeField] GameObject _layout; public GameObject layout => _layout;



        [SerializeField] Sprite downGradation;
        [SerializeField] Sprite upGradation;
        protected override void OnEnable()
        {
            if (SingletonCheck(this))
            {
                InitialLoadManager.initialLoadEnd += AniStart;

                //씬이 이동하고 나서 잠깐 렉이 있기 때문에, 애니메이션이 제대로 재생될려면 딜레이를 걸어줘야합니다
                async void AniStart()
                {
                    BottomMode();

                    aniStop = true;
                    if (await UniTask.DelayFrame(3, cancellationToken: this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow())
                        return;
                    aniStop = false;
                }
            }
        }

        static bool defaultTabAllow = false;
        static GameObject oldSelectedObject;
        static bool tempSelectedStatusBar;
        static bool pointer = false;
        static float timer = 0;
        static bool aniStop = false;
        static bool tempBottomMode = false;
        static bool tempCropTheScreen = true;
        void Update()
        {
            if (InitialLoadManager.isInitialLoadEnd && !aniStop)
            {
                {
                    bool mouseYisScreenY = false;
                    if (InputManager.mousePosition.x >= 0 && InputManager.mousePosition.x <= ScreenManager.width)
                    {
                        if (SaveData.bottomMode)
                            mouseYisScreenY = InputManager.mousePosition.y <= 1;
                        else
                            mouseYisScreenY = InputManager.mousePosition.y >= (ScreenManager.height - 1);
                    }

                    selectedStatusBar = pointer || mouseYisScreenY || SideBarManager.isSideBarShow || (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponentInParent<Kernel>() != null);
                    bool statusBarShow = selectedStatusBar || timer > 0;
                    isStatusBarShow = allowStatusBarShow || statusBarShow;
                    defaultTabAllow = oldSelectedObject == null || !oldSelectedObject.activeInHierarchy || oldSelectedObject.GetComponentInParent<UIManager>() == null;

                    if (selectedStatusBar)
                        timer = 1;
                    else
                        timer -= Kernel.unscaledDeltaTime;

                    if (tempSelectedStatusBar != selectedStatusBar)
                        InputManager.SetInputLock("statusbar", selectedStatusBar);

                    tempSelectedStatusBar = selectedStatusBar;

                    if (selectedStatusBar)
                    {
                        oldSelectedObject = EventSystem.current.currentSelectedGameObject;

                        if (!background.gameObject.activeSelf)
                        {
                            background.gameObject.SetActive(true);
                            background.raycastTarget = true;
                        }

                        background.color = background.color.Lerp(new Color(0, 0, 0, 0.5f), 0.2f * Kernel.fpsUnscaledDeltaTime);
                    }
                    else
                    {
                        if (background.gameObject.activeSelf)
                        {
                            if (background.color.a <= 0.01f)
                            {
                                background.color = new Color(0, 0, 0, 0);
                                background.gameObject.SetActive(false);
                                background.raycastTarget = false;
                            }
                            else
                                background.color = background.color.Lerp(Color.clear, 0.2f * Kernel.fpsUnscaledDeltaTime);
                        }
                    }
                    
                    if ((!selectedStatusBar || (statusBarShow && defaultTabAllow)) && (InputManager.GetKey("gui.tab", InputType.Down, InputManager.inputLockDenyAllForce)))
                        Tab();
                    else if (selectedStatusBar && InputManager.GetKey("gui.back", InputType.Down, InputManager.inputLockDenyAll))
                        EventSystem.current.SetSelectedGameObject(null);
                }



                {
                    if (isStatusBarShow)
                    {
                        rectTransform.anchoredPosition = rectTransform.anchoredPosition.Lerp(Vector2.zero, 0.2f * Kernel.fpsUnscaledDeltaTime);

                        if (!layout.activeSelf)
                        {
                            layout.SetActive(true);
                            graphic.enabled = true;
                        }
                    }
                    else
                    {
                        if (!SaveData.bottomMode)
                        {
                            rectTransform.anchoredPosition = rectTransform.anchoredPosition.Lerp(new Vector2(0, rectTransform.rect.size.y), 0.2f * Kernel.fpsUnscaledDeltaTime);

                            if (rectTransform.anchoredPosition.y >= rectTransform.rect.size.y - 0.01f)
                            {
                                if (layout.activeSelf)
                                {
                                    layout.SetActive(false);
                                    graphic.enabled = false;
                                }
                            }
                        }
                        else
                        {
                            rectTransform.anchoredPosition = rectTransform.anchoredPosition.Lerp(new Vector2(0, -rectTransform.rect.size.y), 0.2f * Kernel.fpsUnscaledDeltaTime);

                            if (rectTransform.anchoredPosition.y <= -rectTransform.rect.size.y + 0.01f)
                            {
                                if (layout.activeSelf)
                                {
                                    layout.SetActive(false);
                                    graphic.enabled = false;
                                }
                            }
                        }
                    }

                    if (backButtonShow && !backButton.activeSelf)
                        backButton.SetActive(true);
                    else if (!backButtonShow && backButton.activeSelf)
                        backButton.SetActive(false);
                }

                {
                    if (tempBottomMode != SaveData.bottomMode)
                    {
                        BottomMode();
                        tempBottomMode = SaveData.bottomMode;
                    }

                    if (tempCropTheScreen != cropTheScreen)
                    {
                        if (!SaveData.bottomMode)
                        {
                            if (!cropTheScreen && !allowStatusBarShow)
                                image.sprite = downGradation;
                            else
                                image.sprite = null;
                        }
                        else
                        {
                            if (!cropTheScreen && !allowStatusBarShow)
                                image.sprite = upGradation;
                            else
                                image.sprite = null;
                        }

                        tempCropTheScreen = cropTheScreen;
                    }
                }

                {
                    Rect rect = Rect.zero;

                    if (cropTheScreen)
                    {
                        if (SaveData.bottomMode)
                            rect.min = new Vector2(0, rectTransform.rect.size.y + rectTransform.anchoredPosition.y);
                        else
                            rect.max = new Vector2(0, -(rectTransform.rect.size.y - rectTransform.anchoredPosition.y));
                    }

                    cropedRect = rect;
                }
            }
        }

        void BottomMode()
        {
            if (!SaveData.bottomMode)
            {
                rectTransform.anchorMin = Vector2.up;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.pivot = Vector2.up;

                if (!isStatusBarShow)
                    rectTransform.anchoredPosition = new Vector2(0, rectTransform.rect.size.y);
                else
                    rectTransform.anchoredPosition = new Vector2(0, 0);
            }
            else
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.right;
                rectTransform.pivot = Vector2.zero;

                if (!isStatusBarShow)
                    rectTransform.anchoredPosition = new Vector2(0, -rectTransform.rect.size.y);
                else
                    rectTransform.anchoredPosition = new Vector2(0, 0);
            }
        }

        public static void Tab()
        {
            if (!instance.layout.activeSelf)
                instance.layout.SetActive(true);
            
            if (tabSelectGameObject != null)
            {
                if (!tabAllow)
                    return;

                EventSystem.current.SetSelectedGameObject(tabSelectGameObject);
                return;
            }

            if (defaultTabAllow)
            {
                Selectable[] selectables = instance.GetComponentsInChildren<Selectable>();
                if (selectables.Length > 0)
                    EventSystem.current.SetSelectedGameObject(selectables[0].gameObject);
                else
                    EventSystem.current.SetSelectedGameObject(instance.gameObject);
            }
            else
                EventSystem.current.SetSelectedGameObject(oldSelectedObject);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => pointer = true;
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => pointer = false;
    }
}