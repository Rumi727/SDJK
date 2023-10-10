using SCKRM;
using SCKRM.Rhythm;
using SDJK.Mode;
using SDJK.Mode.Difficulty;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SDJK.Effect.PostProcessing
{
    public sealed class HPVignette : PostProcessEffect
    {
        public float hp { get; set; } = 0;
        public float maxHp { get; set; } = 0;

        float lerpValue = 0;
        bool? noFail;
        protected override void RealUpdate()
        {
            noFail ??= effectManager.selectedModes.FindMode<NoFailModeBase>() != null;

            float value = (1.5f - 0f.InverseLerpUnclamped(maxHp * 0.35f, hp)).Clamp(0, 1);
            lerpValue = lerpValue.Lerp(value, 0.0625f * RhythmManager.bpmFpsDeltaTime);

            Vignette vignette = profile.GetSetting<Vignette>();

            vignette.active = lerpValue > 0 && (!noFail ?? false);
            vignette.intensity.value = 0.4f * lerpValue;
        }
    }
}
