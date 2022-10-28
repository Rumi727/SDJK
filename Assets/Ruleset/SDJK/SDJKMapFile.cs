using System.Collections.Generic;

namespace SDJK.Ruleset.SDJK.Map
{
    public sealed class SDJKMapFile : global::SDJK.Map.MapFile
    {
        public SDJKMapEffect effect { get; } = new SDJKMapEffect();

        /// <summary>
        /// notes[bar_index][note_index] = note
        /// </summary>
        public List<List<NoteFile>> notes { get; set; } = new List<List<NoteFile>>();
    }

    public struct NoteFile
    {
        public double beat;
        public double length;

        public NoteFile(double beat, double length)
        {
            this.beat = beat;
            this.length = length;
        }
    }

    public sealed class SDJKMapEffect
    {

    }
}
