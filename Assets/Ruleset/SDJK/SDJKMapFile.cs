using System.Collections.Generic;

namespace SDJK.Ruleset.SDJK.Map
{
    public sealed class SDJKMapFile : global::SDJK.Map.Map
    {
        public SDJKMapEffect effect { get; } = new SDJKMapEffect();

        /// <summary>
        /// notes[bar_index][note_index] = note
        /// </summary>
        public List<List<Note>> notes { get; set; } = new List<List<Note>>();
    }

    public struct Note
    {
        public double beat;
        public double length;

        public Note(double beat, double length)
        {
            this.beat = beat;
            this.length = length;
        }
    }

    public sealed class SDJKMapEffect
    {

    }
}
