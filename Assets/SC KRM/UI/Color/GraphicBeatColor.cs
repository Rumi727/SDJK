using SCKRM.Rhythm;
using UnityEngine;

namespace SCKRM.UI
{
    [AddComponentMenu("SC KRM/UI/Color/Graphic Beat Color", 0)]
    public sealed class GraphicBeatColor : UI
    {
        [SerializeField] float _alpha = 1;
        public float alpha { get => _alpha; set => _alpha = value; }

        [SerializeField] bool _dropPartMode = false;
        public bool dropPartMode => _dropPartMode;

        void Update()
        {
            if (RhythmManager.isPlaying && ((RhythmManager.dropPart && dropPartMode) || !dropPartMode))
                graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, alpha.Lerp(0f, (float)RhythmManager.currentBeat1Beat));
            else
                graphic.color = graphic.color.MoveTowards(new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0), 0.025f * Kernel.fpsUnscaledDeltaTime);
        }
    }
}
