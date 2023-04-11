using SCKRM.Rhythm;
using SDJK.Map;
using UnityEngine.Rendering.PostProcessing;

namespace SDJK.Effect.PostProcessing
{
    public sealed class LensDistortionEffect : PostProcessEffect
    {
        protected override void RealUpdate()
        {
            double currentBeat = RhythmManager.currentBeatScreen;

            MapPostProcessLensDistortionEffect lensDistortionEffect = map.postProcessEffect.lensDistortion;
            LensDistortion lensDistortion = profile.GetSetting<LensDistortion>();

            lensDistortion.active = lensDistortionEffect.active.GetValue(currentBeat);
            lensDistortion.intensity.value = lensDistortionEffect.intensity.GetValue(currentBeat);

            lensDistortion.intensityX.value = lensDistortionEffect.intensityX.GetValue(currentBeat);
            lensDistortion.intensityY.value = lensDistortionEffect.intensityY.GetValue(currentBeat);

            lensDistortion.centerX.value = lensDistortionEffect.centerX.GetValue(currentBeat);
            lensDistortion.centerY.value = lensDistortionEffect.centerY.GetValue(currentBeat);

            lensDistortion.scale.value = lensDistortionEffect.scale.GetValue(currentBeat);
        }
    }
}
