namespace SDJK.Mode.Ruleset.SDJK
{
    public sealed class SDJKAutoMode : AutoModeBase
    {
        public override string name { get; } = typeof(SDJKAutoMode).FullName;
        public override string targetRuleset { get; } = "sdjk";
    }
}
