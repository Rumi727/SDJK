using SCKRM.Rhythm;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SDJK.Map
{
    public sealed partial class MapPostProcessEffect
    {
        public MapPostProcessVignetteEffect vignette { get; set; } = new MapPostProcessVignetteEffect();
    }

    public sealed class MapPostProcessVignetteEffect
    {
        public BeatValuePairList<bool> active { get; set; } = new BeatValuePairList<bool>(false);
        public BeatValuePairAniListFloat intensity { get; set; } = new BeatValuePairAniListFloat(0);

        public BeatValuePairList<VignetteMode> mode { get; set; } = new BeatValuePairList<VignetteMode>(VignetteMode.Classic);

        public BeatValuePairAniListColor color { get; set; } = new BeatValuePairAniListColor(Color.black);
        public BeatValuePairAniListVector2 center { get; set; } = new BeatValuePairAniListVector2(new Vector2(0.5f, 0.5f));
        public BeatValuePairAniListFloat smoothness { get; set; } = new BeatValuePairAniListFloat(0.2f);

        public BeatValuePairAniListFloat roundness { get; set; } = new BeatValuePairAniListFloat(1);
        public BeatValuePairList<bool> rounded { get; set; } = new BeatValuePairList<bool>(false);
    }
}
