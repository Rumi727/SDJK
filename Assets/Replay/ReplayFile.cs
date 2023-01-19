using Newtonsoft.Json;
using SCKRM.Rhythm;
using UnityEngine;

namespace SDJK.Replay
{
    public class ReplayFile
    {
        [JsonIgnore] public string replayFilePath { get; set; } = "";

        public BeatValuePairList<KeyCode[]> inputs { get; set; } = new(new KeyCode[0]);

        public BeatValuePairList<double> scores { get; set; } = new(0);
        public BeatValuePairList<double> accuracys { get; set; } = new(0);
        public BeatValuePairList<double> healths { get; set; } = new(0);
    }
}