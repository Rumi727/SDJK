using SCKRM.Renderer;

namespace SDJK.Mode.Automatic
{
    public abstract class AutoModeBase : AutomaticModeBase
    {
        public override int order => int.MaxValue;

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.automatic.auto";
        public override NameSpacePathPair info { get; } = "sdjk:mode.automatic.auto.info";

        public override NameSpaceIndexTypePathPair icon { get; } = "sdjk:0:mode/automatic_auto";
    }
}
