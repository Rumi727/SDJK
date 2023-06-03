using SCKRM;
using SCKRM.Renderer;
using SDJK.Mode.Automatic;
using System;

namespace SDJK.Mode.Difficulty
{
    public abstract class PerfectModeBase : DifficultyModeBase
    {
        public override int order => 2100;

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.difficulty.perfect";
        public override NameSpacePathPair info { get; } = "sdjk:mode.difficulty.perfect.info";

        public override NameSpaceIndexTypePathPair icon { get; } = "sdjk:0:mode/difficulty_perfect";

        public override Type[] incompatibleModes { get; } = new Type[] { typeof(NoFailModeBase), typeof(SuddenDeathModeBase), typeof(AutoModeBase) };



        [ModeConfigSaveLoad, SaveLoadUI("sdjk:mode.difficulty.perfect")]
        public sealed class Config : IModeConfig
        {
            [SaveLoadUIToggleConfig("sc-krm:gui.restart_on_fail", "")]
            public bool restartOnFail { get; set; } = true;
        }

        protected override IModeConfig CreateModeConfig() => new Config();
    }
}
