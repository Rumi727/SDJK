using SCKRM.Rhythm;

namespace SDJK.Replay
{
    public sealed class SDJKReplayFile : ReplayFile
    {
        public BeatValuePairList<int> combos { get; set; } = new(0);
    }
}
