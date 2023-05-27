using System.Collections.Generic;

namespace SDJK.Map.Ruleset.Osu
{
    public class OsuMapFile : MapFile
    {
        public OsuMapFile(string mapFilePath) : base(mapFilePath) { }

        public TypeList<OsuNoteFile> beats { get; } = new();

        public override void FixAllJudgmentBeat()
        {
            TypeList<double> allJudgmentBeat = new TypeList<double>();

            for (int i = 0; i < beats.Count; i++)
            {
                OsuNoteFile note = beats[i];
                allJudgmentBeat.Add(note.beat);

                if (note.holdLength > 0)
                    allJudgmentBeat.Add(note.beat + note.holdLength);
            }

            allJudgmentBeat.Sort();
            this.allJudgmentBeat = allJudgmentBeat;
        }
    }

    public struct OsuNoteFile
    {
        public OsuNoteFile(double beat, double holdLength)
        {
            this.beat = beat;
            this.holdLength = holdLength;

            hitsoundFiles = HitsoundFile.defaultHitsounds;
            holdHitsoundFiles = HitsoundFile.defaultHitsounds;
        }

        public OsuNoteFile(double beat, double holdLength, TypeList<HitsoundFile> hitsoundFiles, TypeList<HitsoundFile> holdHitsoundFiles)
        {
            this.beat = beat;
            this.holdLength = holdLength;

            this.hitsoundFiles = hitsoundFiles;
            this.holdHitsoundFiles = holdHitsoundFiles;
        }

        public double beat { get; set; }
        public double holdLength { get; set; }

        public TypeList<HitsoundFile> hitsoundFiles { get; set; }
        public TypeList<HitsoundFile> holdHitsoundFiles { get; set; }
    }
}
