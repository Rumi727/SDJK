using SCKRM;
using UnityEngine;

namespace SDJK.MainMenu
{
    public class BottomBar : MonoBehaviour
    {
        [SerializeField, NotNull] RectTransform layout;
        [SerializeField, NotNull] CanvasGroup canvasGroup;

        void Update()
        {
            if ((MainMenu.currentScreenMode == ScreenMode.mapPackSelect || MainMenu.currentScreenMode == ScreenMode.mapSelect) && MainMenu.barAlpha <= 0)
            {
                layout.anchoredPosition = layout.anchoredPosition.Lerp(new Vector2(layout.anchoredPosition.x, 0), 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
                canvasGroup.alpha += 0.1f * Kernel.fpsUnscaledSmoothDeltaTime;
            }
            else
            {
                layout.anchoredPosition = layout.anchoredPosition.MoveTowards(new Vector2(layout.anchoredPosition.x, -layout.rect.height), 8 * Kernel.fpsUnscaledSmoothDeltaTime);
                canvasGroup.alpha -= 0.1f * Kernel.fpsUnscaledSmoothDeltaTime;
            }

            if (MainMenu.barAlpha >= 1)
            {
                if (layout.gameObject.activeSelf)
                    layout.gameObject.SetActive(false);
            }
            else if (!layout.gameObject.activeSelf)
                layout.gameObject.SetActive(true);
        }
    }
}
