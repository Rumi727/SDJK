using SCKRM.Rhythm;
using SDJK.Map;
using UnityEngine.Rendering.PostProcessing;

namespace SDJK.Effect.PostProcessing
{
    public sealed class BloomEffect : PostProcessEffect
    {
        protected override void RealUpdate()
        {
            double currentBeat = RhythmManager.currentBeatScreen;

            MapPostProcessBloomEffect bloomEffect = map.postProcessEffect.bloom;
            Bloom bloom = profile.GetSetting<Bloom>();

            bloom.active = bloomEffect.active.GetValue(currentBeat);
            bloom.intensity.value = bloomEffect.intensity.GetValue(currentBeat);

            bloom.threshold.value = bloomEffect.threshold.GetValue(currentBeat);
            bloom.softKnee.value = bloomEffect.softKnee.GetValue(currentBeat);
            bloom.clamp.value = bloomEffect.clamp.GetValue(currentBeat);
            bloom.diffusion.value = bloomEffect.diffusion.GetValue(currentBeat);
            bloom.anamorphicRatio.value = bloomEffect.anamorphicRatio.GetValue(currentBeat);

            bloom.color.value = bloomEffect.color.GetValue(currentBeat);
            bloom.fastMode.value = bloomEffect.fastMode.GetValue(currentBeat);
        }
    }
}
