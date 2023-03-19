using SCKRM.Json;
using SCKRM.Rhythm;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Map.Ruleset.SuperHexagon.Map
{
    public sealed class SuperHexagonMapFile : MapFile
    {
        public SuperHexagonMapFile(string mapFilePath) : base(mapFilePath) { }

        /// <summary>
        /// notes[bar_index][note_index] = note
        /// </summary>
        public TypeList<TypeList<SuperHexagonNoteFile>> notes { get; set; } = new();

        public SuperHexagonEffectFile effect { get; set; } = new SuperHexagonEffectFile();

        public override void SetVisualizerEffect()
        {
            visualizerEffect.divide.Clear();
            for (int i = 0; i < effect.sidesList.Count; i++)
            {
                BeatValuePairAni<double> sides = effect.sidesList[i];
                visualizerEffect.divide.Add(sides.beat, 0, (int)sides.value);
            }

            visualizerEffect.leftMove.Clear();
            for (int i = 0; i < effect.fieldZRotationSpeed.Count; i++)
            {
                BeatValuePairAni<float> fieldZRotationSpeed = effect.fieldZRotationSpeed[i];
                visualizerEffect.leftMove.Add(fieldZRotationSpeed.beat, fieldZRotationSpeed.value >= 0 ? true : false);
            }
        }
    }

    public struct SuperHexagonNoteFile
    {
        public double beat { get; set; }
        public double holdLength { get; set; }

        public SuperHexagonNoteFile(double beat, double holdLength)
        {
            this.beat = beat;
            this.holdLength = holdLength;
        }
    }

    public sealed class SuperHexagonEffectFile
    {
        public BeatValuePairAniListDouble sidesList { get; set; } = new BeatValuePairAniListDouble(6);
        public BeatValuePairAniListDouble playerSpeed { get; set; } = new BeatValuePairAniListDouble(12);

        public BeatValuePairAniListVector3 fieldRotation { get; set; } = new BeatValuePairAniListVector3(JVector3.zero);
        public BeatValuePairAniListFloat fieldZRotationSpeed { get; set; } = new BeatValuePairAniListFloat(0);

        public BeatValuePairAniListColor backgroundColor { get; set; } = new BeatValuePairAniListColor(new Color(0.125f, 0.125f, 0.125f));
        public BeatValuePairAniListColor backgroundColorAlt { get; set; } = new BeatValuePairAniListColor(new Color(0.25f, 0.25f, 0.25f));

        public BeatValuePairAniListColor mainColor { get; set; } = new BeatValuePairAniListColor(Color.white);
        public BeatValuePairAniListColor mainColorAlt { get; set; } = new BeatValuePairAniListColor(new Color(0.875f, 0.875f, 0.875f));

        public TypeList<SuperHexagonBarEffectFile> barEffect { get; set; } = new();

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
