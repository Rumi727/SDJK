using SCKRM.Json;
using SDJK.Map;
using System.Collections.Generic;

namespace SDJK.Ruleset.SDJK.Map
{
    public sealed class SDJKMapFile : MapFile
    {
        /// <summary>
        /// notes[bar_index][note_index] = note
        /// </summary>
        public List<List<NoteFile>> notes { get; } = new List<List<NoteFile>>();

        public int fieldCount { get; set; } = 1;
        public SDJKMapEffect effect { get; } = new SDJKMapEffect();
    }

    public struct NoteFile
    {
        public double beat;
        public double holdLength;

        public NoteFile(double beat, double holdLength)
        {
            this.beat = beat;
            this.holdLength = holdLength;
        }
    }

    public sealed class SDJKMapEffect
    {
        public FieldEffect<BeatValuePairAniListVector3, JVector3> fieldPos { get; } = new(JVector3.zero);
        public FieldEffect<BeatValuePairAniListVector3, JVector3> fieldRotation { get; } = new(JVector3.zero);
        public FieldEffect<BeatValuePairAniListDouble, double> fieldHeight { get; } = new(16);

        public BarEffect<BeatValuePairAniListDouble, double> localNoteDistance { get; } = new(8);

        public BeatValuePairAniListDouble globalNoteDistance { get; } = new(8);
    }

    public class FieldEffect<TList, TListValue> : List<TList>
    {
        public TListValue defaultValue { get; }
        public FieldEffect(TListValue defaultValue) => this.defaultValue = defaultValue;
    }

    public class BarEffect<TList, TListValue> : FieldEffect<List<TList>, TListValue>
    {
        public BarEffect(TListValue defaultValue) : base(defaultValue) { }
    }
}
