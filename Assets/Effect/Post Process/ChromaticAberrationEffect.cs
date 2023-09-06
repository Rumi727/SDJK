using SCKRM.Rhythm;
using SDJK.Map;
using UnityEngine.Rendering.PostProcessing;

namespace SDJK.Effect.PostProcessing
{
    public sealed class ChromaticAberrationEffect : PostProcessEffect
    {
        protected override void RealUpdate()
        {
            double currentBeat = RhythmManager.currentBeatScreen;

            MapPostProcessChromaticAberrationEffect chromaticAberrationEffect = map.postProcessEffect.chromaticAberration;
            ChromaticAberration chromaticAberration = profile.GetSetting<ChromaticAberration>();

            chromaticAberration.active = chromaticAberrationEffect.active.GetValue(currentBeat);
            chromaticAberration.intensity.value = chromaticAberrationEffect.intensity.GetValue(currentBeat);

            chromaticAberration.fastMode.value = chromaticAberrationEffect.fastMode.GetValue(currentBeat);
        }
    }
}
