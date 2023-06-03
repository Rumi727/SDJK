using SCKRM;
using SCKRM.Renderer;
using SDJK.Mode.Automatic;
using System;

namespace SDJK.Mode.Difficulty
{
    public abstract class SuddenDeathModeBase : DifficultyModeBase
    {
        public override int order => 2000;

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.difficulty.sudden_death";
        public override NameSpacePathPair info { get; } = "sdjk:mode.difficulty.sudden_death.info";

        public override NameSpaceIndexTypePathPair icon { get; } = "sdjk:0:mode/difficulty_sudden_death";

        public override Type[] incompatibleModes { get; } = new Type[] { typeof(NoFailModeBase), typeof(PerfectModeBase), typeof(AutoModeBase) };



        [ModeConfigSaveLoad, SaveLoadUI("sdjk:mode.difficulty.sudden_death")]
        public sealed class Config : IModeConfig
        {
            [SaveLoadUIToggleConfig("sc-krm:gui.restart_on_fail", "")]
            public bool restartOnFail { get; set; } = true;
        }

        protected override IModeConfig CreateModeConfig() => new Config();
    }
}
