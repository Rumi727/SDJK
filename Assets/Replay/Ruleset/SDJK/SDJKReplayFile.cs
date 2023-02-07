using SCKRM.Rhythm;
using System.Collections.Generic;

namespace SDJK.Replay
{
    public sealed class SDJKReplayFile : ReplayFile
    {
        public BeatValuePairList<int> combos { get; set; } = new(0);

        public List<List<double>> pressBeat { get; set; } = new();
        public List<List<double>> pressUpBeat { get; set; } = new();
    }
}
