using SDJK.Mode.Converter;

namespace SDJK.Mode.Ruleset.SDJK.Converter
{
    public sealed class SDJKChordjackOffMode : StreamOffModeBase
    {
        public override string targetRuleset { get; } = "sdjk";
    }
}
