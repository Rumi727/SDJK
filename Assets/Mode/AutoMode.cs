using SCKRM.Renderer;

namespace SDJK.Mode
{
    public abstract class AutoModeBase : ModeBase
    {
        public override NameSpacePathReplacePair title { get; } = "sdjk:mode.automatic";
        public override int order => 0;

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.automatic.auto";
        public override NameSpacePathReplacePair info { get; } = "sdjk:mode.automatic.auto.info";

        public override NameSpaceIndexTypePathPair icon { get; } = "sdjk:0:mode/automatic_auto";
    }
}
