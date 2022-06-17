using SCKRM.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SCKRM.UI.SideBar
{
    [AddComponentMenu("SC KRM/Kernel/UI/Side Bar Ani")]
    public sealed class SideBarAni : UIAni
    {
        [SerializeField] string _showControlKey; public string showControlKey => _showControlKey;
        [SerializeField] string _inputLockName; public string inputLockName => _inputLockName;


        [SerializeField] UnityEvent _showEvent; public UnityEvent showEvent => _showEvent;
        [SerializeField] UnityEvent _hideEvent; public UnityEvent hideEvent => _hideEvent;
        [SerializeField] UnityEvent _backEvent; public UnityEvent backEvent => _backEvent;



        /// <summary>
        /// 현제 사이드바가 활성화 되어있는가의 여부
        /// </summary>
        public bool isShow
        {
            get => _isShow;
            private set
            {
                if (value != _isShow)
                {
                    if (value)
                    {
                        SideBarManager.AllHide();

                        UIManager.BackEventAdd(Back, true);
                        UIManager.homeEvent += Hide;

                        SideBarManager.showedSideBars.Add(this);
                    }
                    else
                    {
                        UIManager.BackEventRemove(Back, true);
                        UIManager.homeEvent -= Hide;

                        SideBarManager.showedSideBars.Remove(this);
                    }

                    InputManager.SetInputLock(inputLockName, value);
                    _isShow = value;
                }
            }
        }
        bool _isShow;

        /// <summary>
        /// 사이드바가 오른쪽에 표시되는지에 대한 여부
        /// </summary>
        public bool right { get => _right; set => _right = value; } [SerializeField] bool _right = false;



        #region variable
        public RectTransform viewPort => _viewPort; [SerializeField, NotNull] RectTransform _viewPort;
        public RectTransform content => _content; [SerializeField] RectTransform _content;

        public RectTransform scrollBarParentRectTransform => _scrollBarParentRectTransform; [SerializeField] RectTransform _scrollBarParentRectTransform;
        public Scrollbar scrollBar => _scrollBar; [SerializeField] Scrollbar _scrollBar;
        #endregion

        /// <summary>
        /// 사이드 바를 활성화 합니다
        /// </summary>
        public void Show()
        {
            isShow = true;
            showEvent.Invoke();
        }

        /// <summary>
        /// 사이드 바를 비활성화 합니다
        /// </summary>
        public void Hide()
        {
            isShow = false;
            hideEvent.Invoke();
        }

        /// <summary>
        /// 사이드 바를 활성화/비활성화 합니다
        /// </summary>
        public void Toggle()
        {
            if (isShow)
                Hide();
            else
                Show();
        }

        /// <summary>
        /// 사이드 바를 뒤로 가는 이벤트로 비활성화 합니다
        /// </summary>
        public void Back()
        {
            Hide();
            backEvent.Invoke();
        }

        void Update()
        {
            if (InitialLoadManager.isInitialLoadEnd)
            {
                if (!(showControlKey == null || showControlKey == "") && InputManager.GetKey(showControlKey, InputType.Down, InputManager.inputLockDenyAll))
                    Toggle();

                if (isShow)
                {
                    if (!viewPort.gameObject.activeSelf)
                    {
                        viewPort.gameObject.SetActive(true);
                        graphic.enabled = true;

                        if (scrollBarParentRectTransform != null)
                            scrollBarParentRectTransform.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (this.right)
                    {
                        if (rectTransform.anchoredPosition.x >= rectTransform.rect.size.x - 0.01f)
                        {
                            if (viewPort.gameObject.activeSelf)
                            {
                                viewPort.gameObject.SetActive(false);
                                graphic.enabled = false;

                                if (scrollBarParentRectTransform != null)
                                    scrollBarParentRectTransform.gameObject.SetActive(false);
                            }

                            return;
                        }
                    }
                    else
                    {
                        if (rectTransform.anchoredPosition.x <= (-rectTransform.rect.size.x) + 0.01f)
                        {
                            if (viewPort.gameObject.activeSelf)
                            {
                                viewPort.gameObject.SetActive(false);
                                graphic.enabled = false;

                                if (scrollBarParentRectTransform != null)
                                    scrollBarParentRectTransform.gameObject.SetActive(false);
                            }

                            return;
                        }
                    }
                }


                int right;
                if (this.right)
                    right = 1;
                else
                    right = -1;



                if (lerp)
                {
                    if (isShow)
                        rectTransform.anchoredPosition = rectTransform.anchoredPosition.Lerp(new Vector2(0, rectTransform.anchoredPosition.y), lerpValue * Kernel.fpsUnscaledDeltaTime);
                    else
                        rectTransform.anchoredPosition = rectTransform.anchoredPosition.Lerp(new Vector2(right * rectTransform.rect.size.x, rectTransform.anchoredPosition.y), lerpValue * Kernel.fpsUnscaledDeltaTime);


                    if (scrollBarParentRectTransform != null && scrollBar != null)
                    {
                        if (content.rect.size.y > rectTransform.rect.size.y)
                        {
                            scrollBar.interactable = true;

                            scrollBarParentRectTransform.anchoredPosition = scrollBarParentRectTransform.anchoredPosition.Lerp(Vector2.zero, lerpValue * Kernel.fpsUnscaledDeltaTime);

                            if (this.right)
                                viewPort.offsetMax = viewPort.offsetMax.Lerp(new Vector2(-scrollBarParentRectTransform.rect.size.x, 0), lerpValue * Kernel.fpsUnscaledDeltaTime);
                            else
                                viewPort.offsetMin = viewPort.offsetMin.Lerp(new Vector2(scrollBarParentRectTransform.rect.size.x, 1), lerpValue * Kernel.fpsUnscaledDeltaTime);
                        }
                        else
                        {
                            scrollBar.interactable = false;

                            scrollBarParentRectTransform.anchoredPosition = scrollBarParentRectTransform.anchoredPosition.Lerp(new Vector2(right * scrollBarParentRectTransform.rect.size.x, 0), 0.2f * Kernel.fpsUnscaledDeltaTime);
                            viewPort.offsetMin = viewPort.offsetMin.Lerp(Vector2.zero, lerpValue * Kernel.fpsUnscaledDeltaTime);
                            viewPort.offsetMax = viewPort.offsetMax.Lerp(Vector2.zero, lerpValue * Kernel.fpsUnscaledDeltaTime);
                        }
                    }
                }
                else
                {
                    if (isShow)
                        rectTransform.anchoredPosition = new Vector2(0, rectTransform.anchoredPosition.y);
                    else
                        rectTransform.anchoredPosition = new Vector2(right * rectTransform.rect.size.x, rectTransform.anchoredPosition.y);



                    if (scrollBarParentRectTransform != null && scrollBar != null)
                    {
                        if (content.rect.size.y > rectTransform.rect.size.y)
                        {
                            scrollBar.interactable = true;

                            scrollBarParentRectTransform.anchoredPosition = Vector2.zero;

                            if (this.right)
                                viewPort.offsetMax = new Vector2(-scrollBarParentRectTransform.rect.size.x, 0);
                            else
                                viewPort.offsetMin = new Vector2(scrollBarParentRectTransform.rect.size.x, 1);
                        }
                        else
                        {
                            scrollBar.interactable = false;

                            scrollBarParentRectTransform.anchoredPosition = new Vector2(right * scrollBarParentRectTransform.rect.size.x, 0);
                            viewPort.offsetMin = Vector2.zero;
                            viewPort.offsetMax = Vector2.zero;
                        }
                    }
                }
            }
        }
    }
}
