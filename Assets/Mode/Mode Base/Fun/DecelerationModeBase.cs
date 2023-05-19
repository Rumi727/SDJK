using SCKRM;
using SCKRM.Renderer;
using SDJK.Mode.Difficulty;
using System;

namespace SDJK.Mode.Fun
{
    public abstract class DecelerationModeBase : FunModeBase
    {
        public override int order => 20000;

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.fun.deceleration";
        public override NameSpacePathPair info { get; } = "sdjk:mode.fun.deceleration.info";

        public override NameSpaceIndexTypePathPair icon { get; } = "sdjk:0:mode/fun_deceleration";

        public override Type[] incompatibleModes => new Type[] { typeof(AccelerationModeBase), typeof(AdaptiveSpeedModeBase) };



        [ModeConfigSaveLoad, SaveLoadUI("sdjk:mode.fun.deceleration")]
        public sealed class Config : IModeConfig
        {
            /// <summary>
            /// coefficient per second
            /// </summary>
            [SaveLoadUISliderConfig("gui.coefficient", "gui.coefficient_per_second", 0.001f, 0.01f, 0.001f, 4)]
            public double coefficient { get => _coefficient.Clamp(0); set => _coefficient = value.Clamp(0); }
            double _coefficient = 0.005;

            [SaveLoadUISliderConfig("gui.min", "", 0.0001f, 0.9999f, 0.01f, 4)]
            public double min { get => _min.Clamp(0.0001, 0.9999); set => _min = value.Clamp(0.0001, 0.9999); }
            double _min = 0.25;

            [SaveLoadUIToggleConfig("sdjk:mode.fun.deceleration.resetIfMiss", "")] public bool resetIfMiss { get; set; } = true;
        }

        protected override IModeConfig CreateModeConfig() => new Config();
    }
}
