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



        [ModeConfigSaveLoad, SaveLoadUI("sdjk:mode.converter.hold_off")]
        public sealed class Config : IModeConfig
        {
            [SaveLoadUIToggleConfig("sdjk:mode.converter.hold_off.remove_hold_note_end_beat", "")]
            public bool removeHoldNoteEndBeat { get; set; } = true;
        }

        protected override IModeConfig CreateModeConfig() => new Config();
    }
}
