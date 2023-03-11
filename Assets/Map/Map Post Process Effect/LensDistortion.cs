using SCKRM.Rhythm;

namespace SDJK.Map
{
    public sealed partial class MapPostProcessEffect
    {
        public MapPostProcessLensDistortionEffect lensDistortion { get; set; } = new MapPostProcessLensDistortionEffect();
    }

    public sealed class MapPostProcessLensDistortionEffect
    {
        public BeatValuePairList<bool> active { get; set; } = new BeatValuePairList<bool>(false);
        public BeatValuePairAniListFloat intensity { get; set; } = new BeatValuePairAniListFloat(0);

        public BeatValuePairAniListFloat intensityX { get; set; } = new BeatValuePairAniListFloat(1);
        public BeatValuePairAniListFloat intensityY { get; set; } = new BeatValuePairAniListFloat(1);

        public BeatValuePairAniListFloat centerX { get; set; } = new BeatValuePairAniListFloat(0);
        public BeatValuePairAniListFloat centerY { get; set; } = new BeatValuePairAniListFloat(0);

        public BeatValuePairAniListFloat scale { get; set; } = new BeatValuePairAniListFloat(1);
    }
}
