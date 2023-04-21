using SCKRM;
using SCKRM.Renderer;

namespace SDJK.Mode.Difficulty
{
    public abstract class ComboMultiplierModeBase : DifficultyModeBase
    {
        public override int order => 5100;

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.difficulty.combo_multiplier";
        public override NameSpacePathPair info { get; } = "sdjk:mode.difficulty.combo_multiplier.info";

        public override NameSpaceIndexTypePathPair icon { get; } = "sdjk:0:mode/difficulty_combo_multiplier";



        [ModeConfigSaveLoad, SaveLoadUI("sdjk:mode.difficulty.combo_multiplier")]
        public sealed class Data : IModeConfig
        {
            [SaveLoadUISliderConfig("sdjk:gui.multiplier", "", -2, 2, 0.002f, 4)]
            public double multiplier { get => _multiplier; set => _multiplier = value; }
            double _multiplier = 0.75;
        }

        protected override IModeConfig CreateModeConfig() => new Data();
    }
}
