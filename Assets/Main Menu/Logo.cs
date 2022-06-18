using SCKRM;
using SCKRM.Easing;
using SCKRM.Rhythm;
using SCKRM.Sound;
using SCKRM.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SDJK
{
    public sealed class Logo : UI, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Image image { get => _image = this.GetComponentFieldSave(_image); } [System.NonSerialized] Image _image;

        BeatValuePairAniListFloat beatScaleAni = new BeatValuePairAniListFloat() { new BeatValuePairAni<float>(0, 0.95f, 0, EasingFunction.Ease.Linear), new BeatValuePairAni<float>(0, 1, 0.9, EasingFunction.Ease.EaseOutSine), new BeatValuePairAni<float>(0.9, 0.95f, 0.1, EasingFunction.Ease.Linear) };
        bool pointer = false;
        bool click = false;
        float beatScale = 1;
        float pointerScaleStart = 1;
        float pointerScaleT = 0;
        float pointerScale = 1;
        float clickScale = 1;

        protected override void Awake() => image.alphaHitTestMinimumThreshold = 0.5f;

        int lastCurrentBeat = 0;
        void Update()
        {
            beatScale = beatScaleAni.GetValue(RhythmManager.currentBeat1Beat);

            {
                if (pointerScaleT < 1)
                    pointerScaleT = (pointerScaleT + 0.03f * Kernel.fpsUnscaledDeltaTime).Clamp01();

                if (pointer)
                    pointerScale = (float)EasingFunction.EaseOutElastic(pointerScaleStart, 1.2, pointerScaleT);
                else
                    pointerScale = (float)EasingFunction.EaseOutElastic(pointerScaleStart, 1, pointerScaleT);
            }

            {
                if (click)
                    clickScale = clickScale.Lerp(0.85f, 0.04f * Kernel.fpsUnscaledDeltaTime);
                else
                    clickScale = clickScale.Lerp(1, 0.2f * Kernel.fpsUnscaledDeltaTime);
            }

            transform.localScale = Vector3.one * beatScale * pointerScale * clickScale;

            int currentBeat;
            if (RhythmManager.currentBeat < 0)
                currentBeat = (int)RhythmManager.currentBeat + 1;
            else
                currentBeat = (int)RhythmManager.currentBeat;

            if (lastCurrentBeat != currentBeat)
            {
                if (pointer)
                {
                    if ((int)RhythmManager.currentBeat.Reapeat(4) == 0)
                        SoundManager.PlaySound("hitsound.normal", "sdjk", 0.5f, false, 1.2f);

                    SoundManager.PlaySound("hitsound.normal", "sdjk", 0.5f);
                }

                lastCurrentBeat = currentBeat;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            pointer = true;
            pointerScaleStart = pointerScale;
            pointerScaleT = 0;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            pointer = false;
            pointerScaleStart = pointerScale;
            pointerScaleT = 0;
        }

        public void OnPointerDown(PointerEventData eventData) => click = true;

        public void OnPointerUp(PointerEventData eventData)
        {
            click = false;

            if (pointer)
                MainMenu.NextScreen();
        }
    }
}
