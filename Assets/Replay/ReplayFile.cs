using Newtonsoft.Json;
using SCKRM;
using SCKRM.NTP;
using SCKRM.Rhythm;
using System;

using Version = SCKRM.Version;

namespace SDJK.Replay
{
    public class ReplayFile
    {
        public string mapId { get; set; } = "";

        public Version sckrmVersion { get; set; } = Kernel.sckrmVersion;
        public Version sdjkVersion { get; set; } = new Version(Kernel.version);

        public ReplayModeFile[] modes { get; set; } = new ReplayModeFile[0];

        [JsonIgnore] public string replayFilePath { get; set; } = null;

        public BeatValuePairList<int> combos { get; set; } = new(0);
        public BeatValuePairList<int> maxCombo { get; set; } = new(0);
        public BeatValuePairList<double> scores { get; set; } = new(0);
        public BeatValuePairList<double> accuracyAbses { get; set; } = new(0);
        public BeatValuePairList<double> accuracys { get; set; } = new(0);
        public BeatValuePairList<double> healths { get; set; } = new(0);

        public BeatValuePairList<double> rankProgresses { get; set; } = new(0);

        public int mapMaxCombo { get; set; } = 0;

        public bool isGameOver { get; set; } = false;
        public double gameOverBeat { get; set; } = double.MaxValue;

        [JsonIgnore] public DateTime clearUTCTime { get => new DateTime(clearUTCTimeTick); set => clearUTCTimeTick = value.Ticks; }
        [JsonProperty(nameof(clearUTCTime))] long clearUTCTimeTick = NTPDateTime.utcNow.Ticks;

        public string ruleset { get; set; } = "";
    }
}