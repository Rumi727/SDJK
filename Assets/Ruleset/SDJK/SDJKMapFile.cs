using SDJK.Map;
using System.Collections.Generic;

namespace SDJK.Ruleset.SDJK.Map
{
    public sealed class SDJKMapFile : MapFile
    {
        /// <summary>
        /// notes[bar_index][note_index] = note
        /// </summary>
        public List<List<NoteFile>> notes { get; set; } = new List<List<NoteFile>>();

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

    }
}
