using SCKRM;
using SCKRM.Easing;
using SCKRM.UI;
using SCKRM.UI.Layout;
using SCKRM.UI.StatusBar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK
{
    public sealed class MainMenu : Manager<MainMenu>
    {
        public static ScreenMode currentScreenMode { get; private set; } = ScreenMode.esc;



        [SerializeField] CanvasScaler canvasScaler;
        [SerializeField] RectTransform logo;
        [SerializeField] RectTransform bar;
        [SerializeField] CanvasGroup barCanvasGroup;
        [SerializeField] RectTransform barLayout;
        [SerializeField] HorizontalLayout barLayoutHorizontalLayout;



        void Awake()
        {
            if (SingletonCheck(this))
                StatusBarManager.allowStatusBarShow = false;
        }

        static float screenNormalAniT = 0;
        static Vector2 screenNormalStartPos = Vector2.zero;
        static Vector2 screenNormalStartSize = Vector2.zero;

        static float barAlpha = 0;
        void Update()
        {
            canvasScaler.referenceResolution = new Vector2((ScreenManager.width / UIManager.currentGuiSize).Clamp(1280), (ScreenManager.height / UIManager.currentGuiSize).Clamp(720));

            if (currentScreenMode == ScreenMode.normal)
            {
                #region Logo Ani
                if (screenNormalAniT < 1)
                    screenNormalAniT = (screenNormalAniT + 0.1f * Kernel.fpsUnscaledDeltaTime).Clamp01();
                else
                {
                    if (barAlpha < 1)
                        barAlpha = (barAlpha += 0.1f * Kernel.fpsUnscaledDeltaTime).Clamp01();

                    barCanvasGroup.alpha = barAlpha;
                    barCanvasGroup.blocksRaycasts = true;
                    barCanvasGroup.interactable = true;

                    bar.sizeDelta = new Vector2(0, (float)EasingFunction.EaseOutCubic(0, 135, barAlpha));
                    barLayout.offsetMin = barLayout.offsetMin.Lerp(new Vector2(-210, 0), 0.2f * Kernel.fpsUnscaledDeltaTime);
                    barLayoutHorizontalLayout.spacing = barLayoutHorizontalLayout.spacing.Lerp(0, 0.2f * Kernel.fpsUnscaledDeltaTime);
                }

                float x = (float)EasingFunction.EaseInQuad(screenNormalStartPos.x, -300, screenNormalAniT);
                logo.anchoredPosition = new Vector2(x, 0);

                x = (float)EasingFunction.EaseInQuad(screenNormalStartSize.x, 320, screenNormalAniT);
                float y = (float)EasingFunction.EaseInQuad(screenNormalStartSize.y, 320, screenNormalAniT);
                logo.sizeDelta = new Vector2(x, y);
                #endregion
            }
            else
            {
                #region Logo Ani
                if (barAlpha <= 0)
                {
                    logo.anchoredPosition = logo.anchoredPosition.Lerp(Vector2.zero, 0.2f * Kernel.fpsUnscaledDeltaTime);
                    logo.sizeDelta = logo.sizeDelta.Lerp(Vector2.zero, 0.2f * Kernel.fpsUnscaledDeltaTime);

                    barLayoutHorizontalLayout.spacing = -200;
                    barLayout.offsetMin = new Vector2(-410, 0);
                    screenNormalAniT = 0;
                }
                else
                {
                    if (barAlpha > 0)
                        barAlpha = (barAlpha -= 0.1f * Kernel.fpsUnscaledDeltaTime).Clamp01();

                    barCanvasGroup.alpha = barAlpha;
                    barCanvasGroup.blocksRaycasts = false;
                    barCanvasGroup.interactable = false;

                    bar.sizeDelta = new Vector2(0, (float)EasingFunction.EaseOutCubic(0, 135, barAlpha));
                    barLayout.offsetMin = barLayout.offsetMin.Lerp(new Vector2(-410, 0), 0.2f * Kernel.fpsUnscaledDeltaTime);
                    barLayoutHorizontalLayout.spacing = barLayoutHorizontalLayout.spacing.Lerp(-200, 0.2f * Kernel.fpsUnscaledDeltaTime);
                }
                #endregion
            }
        }

        public static void Esc()
        {
            currentScreenMode = ScreenMode.esc;
            UIManager.BackEventRemove(Esc);

            Vector2 size = instance.logo.rect.size;

            instance.logo.anchorMin = new Vector2(0.5f, 0.2f);
            instance.logo.anchorMax = new Vector2(0.5f, 0.8f);

            instance.logo.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            instance.logo.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

            StatusBarManager.allowStatusBarShow = false;
        }

        public static void Normal()
        {
            currentScreenMode = ScreenMode.normal;
            UIManager.BackEventAdd(Esc);

            Vector2 size = instance.logo.rect.size;

            instance.logo.anchorMin = new Vector2(0.5f, 0.5f);
            instance.logo.anchorMax = new Vector2(0.5f, 0.5f);

            instance.logo.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            instance.logo.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

            screenNormalStartPos = instance.logo.anchoredPosition;
            screenNormalStartSize = instance.logo.sizeDelta;

            StatusBarManager.allowStatusBarShow = true;
        }

        public static void MapSelect()
        {
            
        }
    }

    public enum ScreenMode
    {
        esc,
        normal,
        mapSelect
    }
}
