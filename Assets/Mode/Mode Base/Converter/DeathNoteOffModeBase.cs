using SCKRM;
using SCKRM.Renderer;

namespace SDJK.Mode.Converter
{
    public abstract class DeathNoteOffModeBase : ConverterModeBase
    {
        public override int order => 10200;

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.converter.death_note_off";
        public override NameSpacePathPair info { get; } = "sdjk:mode.converter.death_note_off.info";

        public override NameSpaceIndexTypePathPair icon { get; } = "sdjk:0:mode/converter_death_note_off";
    }
}
