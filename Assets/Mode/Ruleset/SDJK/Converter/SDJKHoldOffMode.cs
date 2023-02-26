using SDJK.Mode.Converter;

namespace SDJK.Mode.Ruleset.SDJK.Converter
{
    public sealed class SDJKHoldOffMode : HoldOffModeBase
    {
        public override string targetRuleset { get; } = "sdjk";
    }
}
