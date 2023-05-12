using System.Collections.Generic;

namespace SDJK.Map.Ruleset.Osu
{
    public class OsuMapFile : MapFile
    {
        public OsuMapFile(string mapFilePath) : base(mapFilePath) { }

        public TypeList<double> beats { get; } = new();

        public override void FixAllJudgmentBeat() => allJudgmentBeat = beats;
    }
}
