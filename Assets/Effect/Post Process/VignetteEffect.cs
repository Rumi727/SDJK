using SCKRM.Rhythm;
using SDJK.Map;
using UnityEngine.Rendering.PostProcessing;

namespace SDJK.Effect.PostProcessing
{
    public sealed class VignetteEffect : PostProcessEffect
    {
        protected override void RealUpdate()
        {
            double currentBeat = RhythmManager.currentBeatScreen;

            MapPostProcessVignetteEffect vignetteEffect = map.postProcessEffect.vignette;
            Vignette vignette = profile.GetSetting<Vignette>();

            vignette.active = vignetteEffect.active.GetValue(currentBeat);
            vignette.intensity.value = vignetteEffect.intensity.GetValue(currentBeat);

            vignette.mode.value = vignetteEffect.mode.GetValue(currentBeat);

            vignette.color.value = vignetteEffect.color.GetValue(currentBeat);
            vignette.center.value = vignetteEffect.center.GetValue(currentBeat);
            vignette.smoothness.value = vignetteEffect.smoothness.GetValue(currentBeat);

            vignette.roundness.value = vignetteEffect.roundness.GetValue(currentBeat);
            vignette.rounded.value = vignetteEffect.rounded.GetValue(currentBeat);
        }
    }
}
