using SCKRM.Input;
using SCKRM.Sound;
using SCKRM.UI.StatusBar;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SCKRM.UI
{
    [AddComponentMenu("SC KRM/UI/Kerenl/Volume Control/Volume Control Manager")]
    public sealed class VolumeControlManager : UIManagerBase<VolumeControlManager>, IPointerEnterHandler, IPointerExitHandler
    {
        static bool isPointer;
        static bool isDrag;
        static float timer = 0;
        [SerializeField] GameObject hide;

        protected override void OnEnable() => SingletonCheck(this);

        void Update()
        {
            if (InitialLoadManager.isInitialLoadEnd)
            {
                if (isPointer || isDrag)
                    timer = 1;
                else
                    timer -= Kernel.unscaledDeltaTime;

                if (isPointer || isDrag || timer >= 0)
                {
                    if (!hide.activeSelf)
                    {
                        hide.SetActive(true);
                        graphic.enabled = true;
                    }

                    if (StatusBarManager.SaveData.bottomMode)
                        rectTransform.anchoredPosition = rectTransform.anchoredPosition.Lerp(new Vector2(rectTransform.anchoredPosition.x, -15), 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
                    else
                        rectTransform.anchoredPosition = rectTransform.anchoredPosition.Lerp(new Vector2(rectTransform.anchoredPosition.x, -15 + StatusBarManager.cropedRect.max.y), 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
                }
                else
                {
                    if (hide.activeSelf && rectTransform.anchoredPosition.y >= rectTransform.rect.size.y - 0.01f)
                    {
                        hide.SetActive(false);
                        graphic.enabled = false;
                    }

                    rectTransform.anchoredPosition = rectTransform.anchoredPosition.Lerp(new Vector2(rectTransform.anchoredPosition.x, rectTransform.rect.size.y + 1), 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
                }

                if (InputManager.GetKey("volume_control.minus", InputType.Down, InputManager.inputLockDenyAllForce))
                {
                    if (isPointer || isDrag || timer >= 0)
                        SoundManager.SaveData.mainVolume -= 10;

                    timer = 1;
                }
                else if (InputManager.GetKey("volume_control.plus", InputType.Down, InputManager.inputLockDenyAllForce))
                {
                    if (isPointer || isDrag || timer >= 0)
                        SoundManager.SaveData.mainVolume += 10;

                    timer = 1;
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData) => isPointer = true;
        public void OnPointerExit(PointerEventData eventData) => isPointer = false;
        public static void OnBeginDrag() => isDrag = true;
        public static void OnEndDrag() => isDrag = false;
    }
}
