using SCKRM;
using SCKRM.Renderer;
using SDJK.Mode.Fun;
using System;

namespace SDJK.Mode.Difficulty
{
    public abstract class SlowModeBase : DifficultyModeBase
    {
        public override int order => 0;

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.difficulty.slow";
        public override NameSpacePathPair info { get; } = "sdjk:mode.difficulty.slow.info";

        public override NameSpaceIndexTypePathPair icon { get; } = "sdjk:0:mode/difficulty_slow";

        public override Type[] incompatibleModes { get; } = new Type[] { typeof(FastModeBase) };



        [ModeConfigSaveLoad, SaveLoadUI("sdjk:mode.difficulty.slow")]
        public sealed class Config : IModeConfig
        {
            [SaveLoadUISliderConfig("sc-krm:gui.speed", "", 0.25f, 1, 0.002f, 4)]
            public double speed { get => _speed.Clamp(0.0001, 1); set => _speed = value.Clamp(0.0001, 1); }
            double _speed = 0.75;

            [SaveLoadUISliderConfig("sc-krm:gui.pitch", "", 0.25f, 1, 0.002f, 4)]
            public double pitch { get => _pitch.Clamp(0.0001, 1); set => _pitch = value.Clamp(0.0001, 1); }
            double _pitch = 1;
        }

        protected override IModeConfig CreateModeConfig() => new Config();
    }
}
