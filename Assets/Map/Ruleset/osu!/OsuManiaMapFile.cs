using System.Collections.Generic;

namespace SDJK.Map.Ruleset.Osu
{
    public sealed class OsuManiaMapFile : OsuMapFile
    {
        public OsuManiaMapFile(string mapFilePath) : base(mapFilePath) { }

        public TypeList<TypeList<OsuManiaNoteFile>> notes { get; } = new();

        public override void FixAllJudgmentBeat()
        {
            TypeList<double> allJudgmentBeat = new TypeList<double>();

            for (int i = 0; i < notes.Count; i++)
            {
                TypeList<OsuManiaNoteFile> notes = this.notes[i];

                for (int j = 0; j < notes.Count; j++)
                {
                    OsuManiaNoteFile note = notes[j];

                    //모든 판정 비트에 노트 추가
                    allJudgmentBeat.Add(note.beat);
                    if (note.holdLength > 0)
                        allJudgmentBeat.Add(note.beat + note.holdLength);
                }
            }

            allJudgmentBeat.Sort();
            this.allJudgmentBeat = allJudgmentBeat;
        }
    }

    public struct OsuManiaNoteFile
    {
        public OsuManiaNoteFile(double beat, double holdLength)
        {
            this.beat = beat;
            this.holdLength = holdLength;
        }

        public double beat { get; set; }
        public double holdLength { get; set; }
    }
}
