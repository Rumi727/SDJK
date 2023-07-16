using SCKRM.Renderer;
using SDJK.Mode.Converter;

namespace SDJK.Mode.Ruleset.SuperHexagon.Converter
{
    public sealed class SuperHexagonKeyCountMode : KeyCountModeBase
    {
        public override string targetRuleset { get; } = "super_hexagon";

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.converter.key_count.super_hexagon";
        public override NameSpacePathPair info { get; } = "sdjk:mode.converter.key_count.super_hexagon.info";
    }
}
