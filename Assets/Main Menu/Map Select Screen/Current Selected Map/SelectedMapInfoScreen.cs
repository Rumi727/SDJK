using SCKRM;
using UnityEngine;

namespace SDJK.MainMenu
{
    public sealed class SelectedMapInfoScreen : MonoBehaviour
    {
        [SerializeField] RectTransform bottom;

        void Update()
        {
            if (MainMenu.currentScreenMode == ScreenMode.mapSelect)
                bottom.anchoredPosition = bottom.anchoredPosition.Lerp(Vector2.zero, 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
            else
                bottom.anchoredPosition = bottom.anchoredPosition.Lerp(new Vector2(0, -bottom.rect.height + 20), 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
        }
    }
}
