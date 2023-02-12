using SCKRM;
using SCKRM.Renderer;

namespace SDJK.Mode
{
    public abstract class ComboMultiplierModeBase : ModeBase
    {
        public override NameSpacePathReplacePair title { get; } = "sdjk:mode.difficulty";
        public override int order => 10000;

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.difficulty.combo_multiplier";
        public override NameSpacePathPair info { get; } = "sdjk:mode.difficulty.combo_multiplier.info";

        public override NameSpaceIndexTypePathPair icon { get; } = "sdjk:0:mode/difficulty_combo_multiplier";



        [ModeConfigSaveLoad, SaveLoadUI("sdjk:mode.difficulty.combo_multiplier")]
        public sealed class Data : IModeConfig
        {
            [SaveLoadUISliderConfig("sdjk:gui.multiplier", "", 0.25f, 2, 0.002f, 4)]
            public double multiplier { get => _multiplier.Clamp(0); set => _multiplier = value.Clamp(0); }
            double _multiplier = 0.25;
        }

        protected override IModeConfig CreateModeConfig() => new Data();
    }
}
