using SCKRM.Json;
using SCKRM.Rhythm;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Map.Ruleset.SuperHexagon.Map
{
    public sealed class SuperHexagonMapFile : MapFile
    {
        /// <summary>
        /// notes[bar_index][note_index] = note
        /// </summary>
        public List<List<SuperHexagonNoteFile>> notes { get; set; } = new List<List<SuperHexagonNoteFile>>();

        public BeatValuePairAniListDouble sides { get; } = new BeatValuePairAniListDouble(6);
        public BeatValuePairAniListDouble playerSpeed { get; } = new BeatValuePairAniListDouble(10);

        public SuperHexagonEffectFile effect { get; set; } = new SuperHexagonEffectFile();
    }

    public struct SuperHexagonNoteFile
    {
        public double beat;
        public double holdLength;

        public SuperHexagonNoteFile(double beat, double holdLength)
        {
            this.beat = beat;
            this.holdLength = holdLength;
        }
    }

    public sealed class SuperHexagonEffectFile
    {
        public BeatValuePairAniListVector3 fieldRotation { get; } = new BeatValuePairAniListVector3(JVector3.zero);
        public BeatValuePairAniListFloat fieldZRotationSpeed { get; } = new BeatValuePairAniListFloat(0);

        public BeatValuePairAniListColor backgroundColor { get; } = new BeatValuePairAniListColor(new Color(0.125f, 0.125f, 0.125f));
        public BeatValuePairAniListColor backgroundColorAlt { get; } = new BeatValuePairAniListColor(new Color(0.25f, 0.25f, 0.25f));

        public BeatValuePairAniListColor mainColor { get; } = new BeatValuePairAniListColor(Color.white);
        public BeatValuePairAniListColor mainColorAlt { get; } = new BeatValuePairAniListColor(new Color(0.875f, 0.875f, 0.875f));
    }
}
