using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public class SDJKMapFile : Map.Map
    {
        /// <summary>
        /// notes[play_field_index][note_index] = note
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
}
