using NodaTime;
using SCKRM;
using SCKRM.Input;
using SCKRM.Rhythm;
using SCKRM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK
{
    public sealed class SkipUI : UIBase
    {
        [SerializeField] Image progress;
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] float alphaAni = 0.15f;

        void Update()
        {
            float t;
            if (RhythmManager.offset > RhythmManager.startDelay)
                t = (float)(-RhythmManager.offset).InverseLerp(-RhythmManager.startDelay, RhythmManager.time);
            else
            {
                if (RhythmManager.time < -RhythmManager.startDelay)
                    t = 0;
                else
                    t = 1;
            }

            if (RhythmManager.isPlaying)
            {
                progress.fillAmount = t.Clamp01();

                if (t >= 1)
                {
                    canvasGroup.alpha = canvasGroup.alpha.MoveTowards(0, alphaAni * Kernel.fpsUnscaledDeltaTime);
                    canvasGroup.blocksRaycasts = false;
                }
                else
                {
                    canvasGroup.alpha = canvasGroup.alpha.MoveTowards(1, alphaAni * Kernel.fpsUnscaledDeltaTime);
                    canvasGroup.blocksRaycasts = true;

                    if (InputManager.GetKey("ruleset.skip"))
                        Skip();
                }
            }
        }

        public void Skip()
        {
            RhythmManager.time = -RhythmManager.startDelay;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
