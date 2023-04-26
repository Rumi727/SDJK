using SCKRM.Json;
using SCKRM.Rhythm;
using System.Collections.Generic;

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
    }

    public struct SDJKNoteFile
    {
        public double beat { get; set; }
        public double holdLength { get; set; }

        public SDJKNoteTypeFile type { get; set; }

        public SDJKNoteFile(double beat, double holdLength, SDJKNoteTypeFile type)
        {
            this.beat = beat;
            this.holdLength = holdLength;

            this.type = type;
        }
    }

    public enum SDJKNoteTypeFile
    {
        normal,
        instantDeath,
        auto
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
