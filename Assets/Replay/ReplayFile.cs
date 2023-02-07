using Newtonsoft.Json;
using SCKRM;
using SCKRM.Rhythm;

namespace SDJK.Replay
{
    public abstract class ReplayFile
    {
        public string mapId { get; set; } = "";

        public Version sckrmVersion { get; set; } = Kernel.sckrmVersion;
        public Version sdjkVersion { get; set; } = new Version(Kernel.version);

        public ReplayModeFile[] modes { get; set; }

        [JsonIgnore] public string replayFilePath { get; set; } = "";

        public BeatValuePairList<int> combos { get; set; } = new(0);
        public BeatValuePairList<int> maxCombo { get; set; } = new(0);
        public BeatValuePairList<double> scores { get; set; } = new(0);
        public BeatValuePairList<double> accuracys { get; set; } = new(0);
        public BeatValuePairList<double> accuracyUnclampeds { get; set; } = new(0);
        public BeatValuePairList<double> healths { get; set; } = new(0);

        public double gameOverBeat { get; set; } = double.MaxValue;
    }
}