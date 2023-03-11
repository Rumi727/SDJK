using SCKRM.Rhythm;
using UnityEngine;

namespace SDJK.Map
{
    public sealed partial class MapPostProcessEffect
    {
        public MapPostProcessBloomEffect bloom { get; set; } = new MapPostProcessBloomEffect();
    }

    public sealed class MapPostProcessBloomEffect
    {
        public BeatValuePairList<bool> active { get; set; } = new BeatValuePairList<bool>(false);
        public BeatValuePairAniListFloat intensity { get; set; } = new BeatValuePairAniListFloat(0);

        public BeatValuePairAniListFloat threshold { get; set; } = new BeatValuePairAniListFloat(1);
        public BeatValuePairAniListFloat softKnee { get; set; } = new BeatValuePairAniListFloat(0.5f);
        public BeatValuePairAniListFloat clamp { get; set; } = new BeatValuePairAniListFloat(65472);
        public BeatValuePairAniListFloat diffusion { get; set; } = new BeatValuePairAniListFloat(7);
        public BeatValuePairAniListFloat anamorphicRatio { get; set; } = new BeatValuePairAniListFloat(0);

        public BeatValuePairAniListColor color { get; set; } = new BeatValuePairAniListColor(Color.white);
        public BeatValuePairList<bool> fastMode { get; set; } = new BeatValuePairList<bool>(false);
    }
}
