using SCKRM;
using SCKRM.Easing;
using SCKRM.Input;
using SCKRM.UI;
using SCKRM.UI.Layout;
using SCKRM.UI.StatusBar;
using System;
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

        void OnEnable() => UIManager.homeEvent += Esc;
        void OnDisable() => UIManager.homeEvent -= Esc;

        static float screenNormalAniT = 0;
        static Vector2 screenNormalStartPos = Vector2.zero;
        static Vector2 screenNormalStartSize = Vector2.zero;

        static float barAlpha = 0;
        void Update()
        {
            canvasScaler.referenceResolution = new Vector2((ScreenManager.width / UIManager.currentGuiSize).Clamp(1280), (ScreenManager.height / UIManager.currentGuiSize).Clamp(720));

            if (InputManager.GetKey(KeyCode.Space) || InputManager.GetKey(KeyCode.Return))
                NextScreen();

            if (currentScreenMode == ScreenMode.esc)
                DefaultLogoAni(Vector2.zero, Vector2.zero);
            else if (currentScreenMode == ScreenMode.normal)
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
                float y = (float)EasingFunction.EaseInQuad(screenNormalStartPos.y, 0, screenNormalAniT);
                logo.anchoredPosition = new Vector2(x, y);

                x = (float)EasingFunction.EaseInQuad(screenNormalStartSize.x, 320, screenNormalAniT);
                y = (float)EasingFunction.EaseInQuad(screenNormalStartSize.y, 320, screenNormalAniT);
                logo.sizeDelta = new Vector2(x, y);
                #endregion
            }
            else if (currentScreenMode == ScreenMode.mapSelect)
                DefaultLogoAni(new Vector2(-92, 50), new Vector2(250, 250));

            bool DefaultLogoAni(Vector2 anchoredPosition, Vector2 sizeDelta)
            {
                if (barAlpha <= 0)
                {
                    logo.anchoredPosition = logo.anchoredPosition.Lerp(anchoredPosition, 0.2f * Kernel.fpsUnscaledDeltaTime);
                    logo.sizeDelta = logo.sizeDelta.Lerp(sizeDelta, 0.2f * Kernel.fpsUnscaledDeltaTime);

                    barLayoutHorizontalLayout.spacing = -200;
                    barLayout.offsetMin = new Vector2(-410, 0);
                    screenNormalAniT = 0;

                    return true;
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

                    return false;
                }
            }
        }

        public static void NextScreen()
        {
            if (currentScreenMode == ScreenMode.esc)
                Normal();
            else if (currentScreenMode == ScreenMode.normal)
                MapSelect();
            else if (currentScreenMode == ScreenMode.mapSelect)
            {

            }
        }

        public static void Esc()
        {
            currentScreenMode = ScreenMode.esc;
            StatusBarManager.allowStatusBarShow = false;

            ScreenChange(new Vector2(0.5f, 0.2f), new Vector2(0.5f, 0.8f));
        }

        public static void Normal()
        {
            currentScreenMode = ScreenMode.normal;
            StatusBarManager.allowStatusBarShow = true;

            ScreenChange(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            UIManager.BackEventAdd(Esc);

            screenNormalStartPos = instance.logo.anchoredPosition;
            screenNormalStartSize = instance.logo.rect.size;
        }

        public static void MapSelect()
        {
            currentScreenMode = ScreenMode.mapSelect;
            StatusBarManager.allowStatusBarShow = true;

            ScreenChange(Vector2.right, Vector2.right);
            UIManager.BackEventAdd(Normal);
        }

        static void ScreenChange(Vector2 anchorMin, Vector2 anchorMax)
        {
            Vector2 pos = instance.logo.localPosition;
            Vector2 size = instance.logo.rect.size;

            instance.logo.anchorMin = anchorMin;
            instance.logo.anchorMax = anchorMax;

            instance.logo.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            instance.logo.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

            instance.logo.localPosition = pos;

            UIManager.BackEventRemove(Esc);
            UIManager.BackEventRemove(Normal);
            UIManager.BackEventRemove(MapSelect);
        }
    }

    public enum ScreenMode
    {
        esc,
        normal,
        mapSelect
    }
}
