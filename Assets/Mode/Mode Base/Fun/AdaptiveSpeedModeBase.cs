using SCKRM;
using SCKRM.Renderer;
using SDJK.Mode.Difficulty;
using System;

namespace SDJK.Mode.Fun
{
    public abstract class AdaptiveSpeedModeBase : FunModeBase
    {
        public override int order => 25000;

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.fun.adaptive_speed";
        public override NameSpacePathPair info { get; } = "sdjk:mode.fun.adaptive_speed.info";

        public override NameSpaceIndexTypePathPair icon { get; } = "sdjk:0:mode/fun_adaptive_speed";

        public override Type[] incompatibleModes => new Type[] { typeof(SlowModeBase), typeof(FastModeBase), typeof(DecelerationModeBase), typeof(AccelerationModeBase) };



        [ModeConfigSaveLoad, SaveLoadUI("sdjk:mode.fun.adaptive_speed")]
        public sealed class Config : IModeConfig
        {
            [SaveLoadUISliderConfig("gui.coefficient", "", 0.01f, 2, 0.01f)]
            public float coefficient { get => _coefficient.Clamp(0); set => _coefficient = value.Clamp(0); }
            float _coefficient = 0.05f;

            [SaveLoadUISliderConfig("gui.min", "", 0.0001f, 0.9999f, 0.01f, 4)]
            public double min { get => _min.Clamp(0.0001, 0.9999); set => _min = value.Clamp(0.0001, 0.9999); }
            double _min = 0.25d;

            [SaveLoadUISliderConfig("gui.max", "", 1.0001f, 2, 0.01f, 4)]
            public double max { get => _max.Clamp(1.0001); set => _max = value.Clamp(1.0001); }
            double _max = 2;
        }

        protected override IModeConfig CreateModeConfig() => new Config();
    }
}
