using SCKRM.Rhythm;
using UnityEngine;

namespace SDJK.Effect
{
    public sealed class VisualizerEffect : Effect
    {
        [SerializeField] Visualizer visualizer;

        public override void Refresh(bool force = false) { }

        int lastDivide = -1;
        bool lastLeftMove = true;
        int lastOffset = -1;
        float lastSize = -1;
        protected override void RealUpdate()
        {
            if (map == null)
                return;

            int divide = map.visualizerEffect.divide.GetValue(RhythmManager.currentBeatScreen);
            bool leftMove = map.visualizerEffect.leftMove.GetValue(RhythmManager.currentBeatScreen);
            int offset = map.visualizerEffect.offset.GetValue(RhythmManager.currentBeatScreen);
            float size = map.visualizerEffect.size.GetValue(RhythmManager.currentBeatScreen);

            if (lastDivide != divide)
            {
                visualizer.divide = divide;
                lastDivide = divide;
            }

            if (lastLeftMove != leftMove)
            {
                visualizer.left = leftMove;
                lastLeftMove = leftMove;
            }

            if (lastOffset != offset)
            {
                visualizer.offset = offset;
                lastOffset = offset;
            }

            if (lastSize != size)
            {
                visualizer.size = size;
                lastSize = size;
            }
        }
    }
}
