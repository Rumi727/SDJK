using SCKRM.Rhythm;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SDJK.Map
{
    public sealed partial class MapPostProcessEffect
    {
        public MapPostProcessChromaticAberrationEffect chromaticAberration { get; set; } = new MapPostProcessChromaticAberrationEffect();
    }

    public sealed class MapPostProcessChromaticAberrationEffect
    {
        public BeatValuePairList<bool> active { get; set; } = new BeatValuePairList<bool>(false);
        public BeatValuePairAniListFloat intensity { get; set; } = new BeatValuePairAniListFloat(0);

        public BeatValuePairList<bool> fastMode { get; set; } = new BeatValuePairList<bool>(false);
    }
}
