using SDJK.Map.Ruleset.SuperHexagon.Map;

namespace SDJK.Ruleset.SDJK.Effect
{
    public abstract class SuperHexagonEffect : global::SDJK.Effect.Effect
    {
        public new SuperHexagonMapFile map => (SuperHexagonMapFile)base.map;
    }
}
