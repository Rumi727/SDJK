using SCKRM.Rhythm;
using SDJK.Map;
using UnityEngine.Rendering.PostProcessing;

namespace SDJK.Effect.PostProcessing
{
    public sealed class GrainEffect : PostProcessEffect
    {
        protected override void RealUpdate()
        {
            double currentBeat = RhythmManager.currentBeatScreen;

            MapPostProcessGrainEffect grainEffect = map.postProcessEffect.grain;
            Grain grain = profile.GetSetting<Grain>();

            grain.active = grainEffect.active.GetValue(currentBeat);
            grain.intensity.value = grainEffect.intensity.GetValue(currentBeat);

            grain.colored.value = grainEffect.colored.GetValue(currentBeat);
            grain.size.value = grainEffect.size.GetValue(currentBeat);
            grain.lumContrib.value = grainEffect.lumContrib.GetValue(currentBeat);
        }
    }
}
