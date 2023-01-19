using SCKRM.Rhythm;
using System.Collections.Generic;

namespace SDJK.Replay
{
    public sealed class SDJKReplayFile : ReplayFile
    {
        public BeatValuePairList<int> combos { get; set; } = new(0);

        public List<List<double>> pressNoteBeat { get; set; } = new();
        public List<List<double>> hitSoundBeat { get; set; } = new();
    }
}
