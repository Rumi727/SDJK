using SCKRM.Renderer;
using SDJK.Mode.Difficulty;

namespace SDJK.Mode.Ruleset.SuperHexagon.Difficulty
{
    public sealed class SuperHexagonSuddenDeathMode : SuddenDeathModeBase
    {
        public override string targetRuleset { get; } = "super_hexagon";

        public override NameSpacePathPair info { get; } = "sdjk:mode.difficulty.sudden_death.super_hexagon.info";
    }
}
