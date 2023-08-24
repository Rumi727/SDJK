using Newtonsoft.Json;
using SCKRM.Json;
using SCKRM.Rhythm;
using System.Collections.Generic;
using System.Linq;

namespace SDJK.Map.Ruleset.SDJK.Map
{
    public sealed class SDJKMapFile : MapFile
    {
        public SDJKMapFile(string mapFilePath) : base(mapFilePath) { }

        /// <summary>
        /// notes[bar_index][note_index] = note
        /// </summary>
        public TypeList<TypeList<SDJKNoteFile>> notes { get; set; } = new TypeList<TypeList<SDJKNoteFile>>();
        public SDJKMapEffectFile effect { get; set; } = new SDJKMapEffectFile();

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

        public override TypeList<double> GetDifficulty() => GetSDJKStyleDifficulty(this, allNotes);

        public static TypeList<double> GetSDJKStyleDifficulty(MapFile mapFile, TypeList<SDJKAllNoteFile> allNotes)
        {
            TypeList<double> diff = new TypeList<double>();
            if (allNotes.Count <= 0)
                return diff;

            double holdBeat = 0;
            for (int i = 0; i < allNotes.Count - 1; i++)
            {
                SDJKAllNoteFile allNote = allNotes[i];

                double size = 0.33;
                if (allNote.holdLength > 0)
                    size *= 1.25;

                if (allNote.beat < holdBeat)
                    size *= 2;

                double? result = DifficultyCalculation(mapFile, allNote.beat, allNotes[i + 1].beat, size);
                if (result != null)
                    diff.Add((double)result);

                if (allNote.holdLength > 0 && allNote.beat >= holdBeat)
                    holdBeat = allNote.beat + allNote.holdLength;
            }

            return diff;
        }

        public override void FixAllJudgmentBeat()
        {
            TypeList<double> allJudgmentBeat = new TypeList<double>();
            TypeList<SDJKAllNoteFile> allNotes = new TypeList<SDJKAllNoteFile>();

            for (int i = 0; i < notes.Count; i++)
            {
                TypeList<SDJKNoteFile> notes = this.notes[i];

                for (int j = 0; j < notes.Count; j++)
                {
                    SDJKNoteFile note = notes[j];

                    //모든 판정 비트에 노트 추가
                    if (note.type != SDJKNoteTypeFile.instantDeath)
                    {
                        allJudgmentBeat.Add(note.beat);
                        if (note.holdLength > 0)
                            allJudgmentBeat.Add(note.beat + note.holdLength);

                        allNotes.Add(new SDJKAllNoteFile(note.beat, note.holdLength, i, j));
                    }
                }
            }

            allJudgmentBeat.Sort();
            this.allJudgmentBeat = allJudgmentBeat;

            this.allNotes = allNotes.OrderBy(x => x.beat).ToTypeList();
        }
    }

    public struct SDJKNoteFile
    {
        public double beat { get; set; }
        public double holdLength { get; set; }

        public SDJKNoteTypeFile type { get; set; }

        public TypeList<HitsoundFile> hitsoundFiles { get => _hitsoundFiles ??= HitsoundFile.defaultHitsounds; set => _hitsoundFiles = value; }
        TypeList<HitsoundFile> _hitsoundFiles;

        public TypeList<HitsoundFile> holdHitsoundFiles { get => _holdHitsoundFiles ??= HitsoundFile.defaultHitsounds; set => _holdHitsoundFiles = value; }
        TypeList<HitsoundFile> _holdHitsoundFiles;

        public SDJKNoteFile(double beat, double holdLength, SDJKNoteTypeFile type)
        {
            this.beat = beat;
            this.holdLength = holdLength;

            this.type = type;

            _hitsoundFiles = HitsoundFile.defaultHitsounds;
            _holdHitsoundFiles = HitsoundFile.defaultHitsounds;
        }

        public SDJKNoteFile(double beat, double holdLength, SDJKNoteTypeFile type, TypeList<HitsoundFile> hitsoundFiles, TypeList<HitsoundFile> holdHitsoundFiles)
        {
            this.beat = beat;
            this.holdLength = holdLength;

            this.type = type;

            _hitsoundFiles = hitsoundFiles;
            _holdHitsoundFiles = holdHitsoundFiles;
        }
    }

    public enum SDJKNoteTypeFile
    {
        normal,
        instantDeath,
        auto
    }

    public readonly struct SDJKAllNoteFile
    {
        public double beat { get; }
        public double holdLength { get; }

        public int keyIndex { get; }
        public int index { get; }

        public SDJKAllNoteFile(double beat, double holdLength, int keyIndex, int index)
        {
            this.beat = beat;
            this.holdLength = holdLength;

            this.keyIndex = keyIndex;
            this.index = index;
        }
    }

    public sealed class SDJKMapEffectFile
    {
        public TypeList<SDJKFieldEffectFile> fieldEffect { get; set; } = new();

        public BeatValuePairAniListDouble globalNoteDistance { get; set; } = new(8);
        /// <summary>
        /// 현재 비트가 아닌 노트의 비트를 기준으로 이펙트를 재생시켜야합니다
        /// </summary>
        public BeatValuePairList<double> globalNoteSpeed { get; set; } = new(1);
    }

    public sealed class SDJKFieldEffectFile
    {
        public BeatValuePairAniListVector3 pos { get; set; } = new(JVector3.zero);
        public BeatValuePairAniListVector3 scale { get; set; } = new(new JVector3(1));
        public BeatValuePairAniListVector3 rotation { get; set; } = new(JVector3.zero);

        public BeatValuePairAniListDouble height { get; set; } = new(16);

        public TypeList<SDJKBarEffectFile> barEffect { get; set; } = new();

        public BeatValuePairAniListDouble noteDistance { get; set; } = new(1);
    }

    public sealed class SDJKBarEffectFile
    {
        public BeatValuePairAniListVector3 pos { get; set; } = new(JVector3.zero);
        public BeatValuePairAniListVector3 scale { get; set; } = new(new JVector3(1));
        public BeatValuePairAniListVector3 rotation { get; set; } = new(JVector3.zero);

        public BeatValuePairAniListColor color { get; set; } = new(JColor.one);
        public BeatValuePairAniListColor noteColor { get; set; } = new(new JColor(0, 1, 0));

        public BeatValuePairAniListDouble noteDistance { get; set; } = new(1);

        public BeatValuePairList<NoteConfigFile> noteConfig { get; set; } = new(new NoteConfigFile());

        public BeatValuePairList<bool> noteStop { get; set; } = new(false);
        public BeatValuePairAniListDouble noteOffset { get; set; } = new(0);
    }
}
