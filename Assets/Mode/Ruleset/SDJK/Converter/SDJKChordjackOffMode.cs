using SDJK.Mode.Converter;

namespace SDJK.Mode.Ruleset.SDJK.Converter
{
    public sealed class SDJKChordjackOffMode : ChordjackOffModeBase
    {
        public override string targetRuleset { get; } = "sdjk";
    }
}
