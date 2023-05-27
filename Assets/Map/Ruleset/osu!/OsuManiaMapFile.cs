using SCKRM.Rhythm;
using System.Collections.Generic;

namespace SDJK.Map.Ruleset.Osu
{
    public sealed class OsuManiaMapFile : OsuMapFile
    {
        public OsuManiaMapFile(string mapFilePath) : base(mapFilePath) { }

        public TypeList<TypeList<OsuNoteFile>> notes { get; } = new();



        public override void FixAllJudgmentBeat()
        {
            TypeList<double> allJudgmentBeat = new TypeList<double>();

            for (int i = 0; i < notes.Count; i++)
            {
                TypeList<OsuNoteFile> notes = this.notes[i];

                for (int j = 0; j < notes.Count; j++)
                {
                    OsuNoteFile note = notes[j];

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
}
