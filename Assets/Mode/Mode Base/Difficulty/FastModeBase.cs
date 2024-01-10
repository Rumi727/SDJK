using SCKRM;
using SCKRM.Renderer;
using SDJK.Mode.Fun;
using System;

namespace SDJK.Mode.Difficulty
{
    public abstract class FastModeBase : DifficultyModeBase
    {
        public override int order => 100;

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.difficulty.fast";
        public override NameSpacePathPair info { get; } = "sdjk:mode.difficulty.fast.info";

        public override NameSpaceIndexTypePathPair icon { get; } = "sdjk:0:mode/difficulty_fast";

        public override Type[] incompatibleModes { get; } = new Type[] { typeof(SlowModeBase) };



        [ModeConfigSaveLoad, SaveLoadUI("sdjk:mode.difficulty.fast")]
        public sealed class Config : IModeConfig
        {
            [SaveLoadUISliderConfig("sc-krm:gui.speed", "", 1, 4, 0.01f, 4)]
            public double speed { get => _speed.Clamp(1); set => _speed = value.Clamp(1); }
            double _speed = 1.5;

            [SaveLoadUISliderConfig("sc-krm:gui.pitch", "", 1, 4, 0.01f, 4)]
            public double pitch { get => _pitch.Clamp(1); set => _pitch = value.Clamp(1); }
            double _pitch = 1;
        }

        protected override IModeConfig CreateModeConfig() => new Config();
    }
}
