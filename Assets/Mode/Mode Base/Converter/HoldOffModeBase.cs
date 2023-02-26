using SCKRM;
using SCKRM.Renderer;

namespace SDJK.Mode.Converter
{
    public abstract class HoldOffModeBase : ConverterModeBase
    {
        public override int order => 10100;

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.converter.hold_off";
        public override NameSpacePathPair info { get; } = "sdjk:mode.converter.hold_off.info";

        public override NameSpaceIndexTypePathPair icon { get; } = "sdjk:0:mode/converter_hold_off";
    }
}
