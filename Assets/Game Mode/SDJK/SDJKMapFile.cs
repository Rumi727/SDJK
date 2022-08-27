using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK
{
    public class SDJKMapFile : Map.Map
    {
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
