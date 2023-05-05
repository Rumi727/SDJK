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
            [SaveLoadUISliderConfig("sc-krm:gui.count", "", 1, 8, 0.01f)]
            public int count { get => _count.Clamp(1); set => _count = value.Clamp(1); }
            int _count = 4;
        }

        protected override IModeConfig CreateModeConfig() => new Data();
    }
}
