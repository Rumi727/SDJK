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

        public BeatValuePairAniListDouble sides { get; set; } = new BeatValuePairAniListDouble(6);
        public BeatValuePairAniListDouble playerSpeed { get; set; } = new BeatValuePairAniListDouble(12);

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
        public BeatValuePairAniListVector3 fieldRotation { get; set; } = new BeatValuePairAniListVector3(JVector3.zero);
        public BeatValuePairAniListFloat fieldZRotationSpeed { get; set; } = new BeatValuePairAniListFloat(0);

        public BeatValuePairAniListColor backgroundColor { get; set; } = new BeatValuePairAniListColor(new Color(0.125f, 0.125f, 0.125f));
        public BeatValuePairAniListColor backgroundColorAlt { get; set; } = new BeatValuePairAniListColor(new Color(0.25f, 0.25f, 0.25f));

        public BeatValuePairAniListColor mainColor { get; set; } = new BeatValuePairAniListColor(Color.white);
        public BeatValuePairAniListColor mainColorAlt { get; set; } = new BeatValuePairAniListColor(new Color(0.875f, 0.875f, 0.875f));

        public List<SuperHexagonBarEffectFile> barEffect { get; set; } = new();

        public BeatValuePairAniListDouble globalNoteDistance { get; set; } = new(8);
        /// <summary>
        /// 현재 비트가 아닌 노트의 비트를 기준으로 이펙트를 재생시켜야합니다
        /// </summary>
        public BeatValuePairList<double> globalNoteSpeed { get; set; } = new(1);
    }

    public sealed class SuperHexagonBarEffectFile
    {
        public BeatValuePairAniListDouble noteDistance { get; set; } = new(1);
        /// <summary>
        /// 현재 비트가 아닌 노트의 비트를 기준으로 이펙트를 재생시켜야합니다
        /// </summary>
        public BeatValuePairList<double> noteSpeed { get; set; } = new(1);
    }
}
