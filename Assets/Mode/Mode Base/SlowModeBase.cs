using SCKRM;
using SCKRM.Renderer;
using System;

namespace SDJK.Mode
{
    public abstract class SlowModeBase : ModeBase
    {
        public override NameSpacePathReplacePair title { get; } = "sdjk:mode.difficulty";
        public override int order => 0;

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.difficulty.slow";
        public override NameSpacePathPair info { get; } = "sdjk:mode.difficulty.slow.info";

        public override NameSpaceIndexTypePathPair icon { get; } = "sdjk:0:mode/difficulty_slow";

        public override Type[] incompatibleModes { get; } = new Type[] { typeof(FastModeBase) };



        [ModeConfigSaveLoad, SaveLoadUI("sdjk:mode.difficulty.slow")]
        public sealed class Data : IModeConfig
        {
            [SaveLoadUISliderConfig("sdjk:gui.speed", "", 0.25f, 0.9999f, 0.002f, 4)]
            public double speed { get => _speed.Clamp(0.0001, 0.9999); set => _speed = value.Clamp(0.0001, 0.9999); }
            double _speed = 0.75;
        }

        protected override IModeConfig CreateModeConfig() => new Data();
    }
}
