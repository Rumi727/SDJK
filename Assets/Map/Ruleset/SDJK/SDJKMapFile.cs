using SCKRM.Json;
using SCKRM.Rhythm;
using System.Collections.Generic;

namespace SDJK.Map.Ruleset.SDJK.Map
{
    public sealed class SDJKMapFile : MapFile
    {
        /// <summary>
        /// notes[bar_index][note_index] = note
        /// </summary>
        public List<List<NoteFile>> notes { get; set; } = new List<List<NoteFile>>();

        public SDJKMapEffectFile effect { get; set; } = new SDJKMapEffectFile();
    }

    public struct NoteFile
    {
        public double beat;
        public double holdLength;

        public NoteTypeFile type;

        public NoteFile(double beat, double holdLength, NoteTypeFile type)
        {
            this.beat = beat;
            this.holdLength = holdLength;

            this.type = type;
        }
    }

    public enum NoteTypeFile
    {
        normal,
        instantDeath,
        auto
    }

    public sealed class SDJKMapEffectFile
    {
        public List<FieldEffectFile> fieldEffect { get; } = new();

        public BeatValuePairAniListDouble globalNoteDistance { get; } = new(8);
        /// <summary>
        /// 현재 비트가 아닌 노트의 비트를 기준으로 이펙트를 재생시켜야합니다
        /// </summary>
        public BeatValuePairList<double> globalNoteSpeed { get; } = new(1);
    }

    public sealed class FieldEffectFile
    {
        public BeatValuePairAniListVector3 pos { get; } = new(JVector3.zero);
        public BeatValuePairAniListVector3 scale { get; } = new(new JVector3(1));
        public BeatValuePairAniListVector3 rotation { get; } = new(JVector3.zero);

        public BeatValuePairAniListDouble height { get; } = new(16);

        public List<BarEffectFile> barEffect { get; } = new();

        public BeatValuePairAniListDouble noteDistance { get; } = new(1);
        /// <summary>
        /// 현재 비트가 아닌 노트의 비트를 기준으로 이펙트를 재생시켜야합니다
        /// </summary>
        public BeatValuePairList<double> noteSpeed { get; } = new(1);
    }

    public sealed class BarEffectFile
    {
        public BeatValuePairAniListVector3 pos { get; } = new(JVector3.zero);
        public BeatValuePairAniListVector3 scale { get; } = new(new JVector3(1));
        public BeatValuePairAniListVector3 rotation { get; } = new(JVector3.zero);

        public BeatValuePairAniListColor color { get; } = new(JColor.one);
        public BeatValuePairAniListColor noteColor { get; } = new(new JColor(0, 1, 0));

        public BeatValuePairAniListDouble noteDistance { get; } = new(1);
        /// <summary>
        /// 현재 비트가 아닌 노트의 비트를 기준으로 이펙트를 재생시켜야합니다
        /// </summary>
        public BeatValuePairList<double> noteSpeed { get; } = new(1);

        public BeatValuePairList<bool> noteStop { get; } = new(false);
        public BeatValuePairAniListDouble noteOffset { get; } = new(0);
    }
}
