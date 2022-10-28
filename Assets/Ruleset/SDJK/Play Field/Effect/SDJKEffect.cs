using SDJK.Ruleset.SDJK.Map;

namespace SDJK.Ruleset.SDJK.Effect
{
    public abstract class SDJKEffect : global::SDJK.Effect.Effect
    {
        public new SDJKMapFile map => (SDJKMapFile)base.map;
    }
}
