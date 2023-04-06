using SCKRM.Renderer;

namespace SDJK.Mode.Difficulty
{
    public abstract class PinpointAccuracyModeBase : DifficultyModeBase
    {
        public override int order => 200;

        public override NameSpacePathReplacePair displayName { get; } = "sdjk:mode.difficulty.pinpoint_accuracy";
        public override NameSpacePathPair info { get; } = "sdjk:mode.difficulty.pinpoint_accuracy.info";

        public override NameSpaceIndexTypePathPair icon { get; } = "sdjk:0:mode/difficulty_pinpoint_accuracy";
    }
}
