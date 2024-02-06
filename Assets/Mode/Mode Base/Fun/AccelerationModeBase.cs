using SCKRM;
using SCKRM.Renderer;
using SDJK.Mode.Difficulty;
using System;

namespace SDJK.Mode.Fun
{
    public abstract class AccelerationModeBase : FunModeBase
    {
        public override int order => 20100;

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.fun.acceleration";
        public override NameSpacePathPair info { get; } = "sdjk:mode.fun.acceleration.info";

        public override NameSpaceIndexTypePathPair icon { get; } = "sdjk:0:mode/fun_acceleration";

        public override Type[] incompatibleModes => new Type[] { typeof(DecelerationModeBase), typeof(AdaptiveSpeedModeBase) };



        [ModeConfigSaveLoad, SaveLoadUI("sdjk:mode.fun.acceleration")]
        public sealed class Config : IModeConfig
        {
            /// <summary>
            /// coefficient per second
            /// </summary>
            [SaveLoadUISliderConfig("gui.coefficient", "gui.coefficient_per_second", 0.001f, 0.01f, 0.001f, 4)]
            public double coefficient { get => _coefficient.Clamp(0); set => _coefficient = value.Clamp(0); }
            double _coefficient = 0.005;

            [SaveLoadUISliderConfig("gui.max", "", 1.0001f, 4, 0.01f, 4)]
            public double max { get => _max.Clamp(1.0001); set => _max = value.Clamp(1.0001); }
            double _max = 2;

            [SaveLoadUIToggleConfig("sdjk:mode.fun.acceleration.resetIfMiss", "")] public bool resetIfMiss { get; set; } = true;

            [SaveLoadUIToggleConfig("gui.pitch", "")] public bool changePitch { get; set; } = true;
        }

        protected override IModeConfig CreateModeConfig() => new Config();
    }
}
