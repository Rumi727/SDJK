using SCKRM.Rhythm;
using UnityEngine;

namespace SDJK.Map
{
    public sealed partial class MapPostProcessEffect
    {
        public MapPostProcessGrainEffect grain { get; set; } = new MapPostProcessGrainEffect();
    }

    public sealed class MapPostProcessGrainEffect
    {
        public BeatValuePairList<bool> active { get; set; } = new BeatValuePairList<bool>(false);
        public BeatValuePairAniListFloat intensity { get; set; } = new BeatValuePairAniListFloat(0);

        public BeatValuePairList<bool> colored { get; set; } = new BeatValuePairList<bool>(false);
        public BeatValuePairAniListFloat size { get; set; } = new BeatValuePairAniListFloat(1);
        public BeatValuePairAniListFloat lumContrib { get; set; } = new BeatValuePairAniListFloat(0.8f);
    }
}
