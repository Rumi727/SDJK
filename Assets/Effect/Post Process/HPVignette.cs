using SCKRM;
using SCKRM.Rhythm;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SDJK.Effect.PostProcessing
{
    public sealed class HPVignette : PostProcessEffect
    {
        public float hp { get; set; } = 0;
        public float maxHp { get; set; } = 0;

        float lerpValue = 0;
        protected override void RealUpdate()
        {
            float value = 1 - 0f.InverseLerp(maxHp * 0.5f, hp);
            lerpValue = lerpValue.Lerp(value, 0.0625f * RhythmManager.bpmFpsDeltaTime);

            Vignette vignette = profile.GetSetting<Vignette>();

            vignette.active = lerpValue > 0;
            vignette.intensity.value = 0.5f * lerpValue;
        }
    }
}
