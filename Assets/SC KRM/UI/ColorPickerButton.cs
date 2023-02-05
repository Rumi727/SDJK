using HSVPicker;
using SCKRM.Cursor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SCKRM.UI
{
    [AddComponentMenu("SC KRM/UI/Color Picker Button")]
    public sealed class ColorPickerButton : UIAni, IPointerEnterHandler, IPointerExitHandler
    {
        bool _isShow = false;
        public bool isShow
        {
            get => _isShow;
            set
            {
                if (value)
                    Toggle();
                else
                    Hide();
            }
        }

        [SerializeField, NotNull] RectTransform colorPickerMask;
        [SerializeField, NotNull] RectTransform colorPickerRectTransform;
        [SerializeField, NotNull] ColorPicker colorPicker;

        bool pointer;



        void Update()
        {
            if (Kernel.isPlaying)
            {
                if (!isShow)
                {
                    if (lerp)
                        colorPickerMask.sizeDelta = colorPickerMask.sizeDelta.Lerp(new Vector2(colorPickerMask.sizeDelta.x, colorPickerMask.anchoredPosition.y), lerpValue * Kernel.fpsUnscaledSmoothDeltaTime);
                    else
                        colorPickerMask.sizeDelta = new Vector2(colorPickerMask.sizeDelta.x, 0);

                    if (colorPickerMask.gameObject.activeSelf && colorPickerMask.sizeDelta.y < colorPickerMask.anchoredPosition.y + 0.01f)
                        colorPickerMask.gameObject.SetActive(false);
                }
                else if (!pointer && !CursorManager.isDragged && UnityEngine.Input.GetMouseButtonUp(0))
                    Hide();

                if (isShow)
                {
                    if (lerp)
                        colorPickerMask.sizeDelta = colorPickerMask.sizeDelta.Lerp(new Vector2(colorPickerMask.sizeDelta.x, colorPickerRectTransform.sizeDelta.y), lerpValue * Kernel.fpsUnscaledSmoothDeltaTime);
                    else
                        colorPickerMask.sizeDelta = new Vector2(colorPickerMask.sizeDelta.x, colorPickerRectTransform.sizeDelta.y);

                    if (!colorPickerMask.gameObject.activeSelf)
                        colorPickerMask.gameObject.SetActive(true);
                }
            }
        }

        protected override void OnEnable() => Hide();

        protected override void OnDisable() => Hide();

        public void Toggle()
        {
            if (isShow)
            {
                Hide();
                return;
            }

            UIManager.BackEventAdd(Hide, true);
            UIManager.homeEvent += Hide;

            _isShow = true;

            if (!lerp)
                Update();
        }

        public void Hide()
        {
            if (!isShow)
                return;

            UIManager.BackEventRemove(Hide, true);
            UIManager.homeEvent -= Hide;

            _isShow = false;

            if (!lerp)
                Update();
        }

        public void OnPointerEnter(PointerEventData eventData) => pointer = true;

        public void OnPointerExit(PointerEventData eventData) => pointer = false;
    }
}