using SCKRM;
using SCKRM.Renderer;

namespace SDJK.Mode.Converter
{
    public abstract class KeyCountModeBase : ConverterModeBase
    {
        public override int order => 10000;

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.converter.key_count";
        public override NameSpacePathPair info { get; } = "sdjk:mode.converter.key_count.info";

        public override NameSpaceIndexTypePathPair icon { get; } = "sdjk:0:mode/converter_key_count";



        [ModeConfigSaveLoad, SaveLoadUI("sdjk:mode.converter.key_count")]
        public sealed class Data : IModeConfig
        {
            [SaveLoadUISliderConfig("sdjk:gui.count", "", 1, 10, 0.01f)]
            public int multiplier { get => _multiplier.Min(1); set => _multiplier = value.Min(1); }
            int _multiplier = 4;
        }

        protected override IModeConfig CreateModeConfig() => new Data();
    }
}
