using System.Collections.Generic;

namespace SDJK.Replay.Ruleset.SDJK
{
    public sealed class SDJKReplayFile : ReplayFile
    {
        public List<List<double>> pressBeat { get; set; } = new();
        public List<List<double>> pressUpBeat { get; set; } = new();
    }
}
