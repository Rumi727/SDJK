using Newtonsoft.Json;
using SDJK.Map.Ruleset.SDJK.Map;
using System.Collections.Generic;
using System.Linq;

namespace SDJK.Map.Ruleset.Osu
{
    public sealed class OsuManiaMapFile : OsuMapFile
    {
        public OsuManiaMapFile(string mapFilePath) : base(mapFilePath) { }

        public TypeList<TypeList<OsuNoteFile>> notes { get; } = new();



        [JsonIgnore]
        public TypeList<SDJKAllNoteFile> allNotes
        {
            get
            {
                if (_allNotes == null)
                    FixAllJudgmentBeat();

                return _allNotes;
            }
            set => _allNotes = value;
        }
        [JsonIgnore] TypeList<SDJKAllNoteFile> _allNotes = null;

        public override TypeList<double> GetDifficulty() => SDJKMapFile.GetSDJKStyleDifficulty(this, allNotes);



        public override void FixAllJudgmentBeat()
        {
            TypeList<double> allJudgmentBeat = new TypeList<double>();
            TypeList<SDJKAllNoteFile> allNotes = new TypeList<SDJKAllNoteFile>();

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

                    allNotes.Add(new SDJKAllNoteFile(note.beat, note.holdLength, i, j));
                }
            }

            allJudgmentBeat.Sort();
            this.allJudgmentBeat = allJudgmentBeat;

            this.allNotes = allNotes.OrderBy(x => x.beat).ToTypeList();
        }
    }
}
