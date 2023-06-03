using SCKRM.Renderer;
using SDJK.Mode.Automatic;
using System;

namespace SDJK.Mode.Difficulty
{
    public abstract class NoFailModeBase : DifficultyModeBase
    {
        public override int order => 5000;

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.difficulty.no_fail";
        public override NameSpacePathPair info { get; } = "sdjk:mode.difficulty.no_fail.info";

        public override NameSpaceIndexTypePathPair icon { get; } = "sdjk:0:mode/difficulty_no_fail";

        public override Type[] incompatibleModes { get; } = new Type[] { typeof(SuddenDeathModeBase), typeof(PerfectModeBase), typeof(AutoModeBase) };
    }
}
