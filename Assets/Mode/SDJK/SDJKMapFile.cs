using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK
{
    public class SDJKMapFile : Map.Map
    {
        public List<Note> notes = new List<Note>();
    }

    public struct Note
    {
        public int index;

        public double beat;
        public double length;

        public Note(int index, double beat, double length)
        {
            this.index = index;

            this.beat = beat;
            this.length = length;
        }
    }
}
