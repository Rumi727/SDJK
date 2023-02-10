using SCKRM.Rhythm;
using UnityEngine;

namespace SCKRM.UI
{
    [AddComponentMenu("SC KRM/UI/Color/Graphic Beat Color", 0)]
    public sealed class GraphicBeatColor : UIBase
    {
        [SerializeField] float _alpha = 1;
        public float alpha { get => _alpha; set => _alpha = value; }

        [SerializeField] bool _yukiModeMode = false;
        public bool yukiModeMode => _yukiModeMode;

        void Update()
        {
            if (RhythmManager.isPlaying && ((RhythmManager.screenYukiMode && yukiModeMode) || !yukiModeMode))
                graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, alpha.Lerp(0f, (float)RhythmManager.currentBeatScreen1Beat));
            else
                graphic.color = graphic.color.MoveTowards(new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0), 0.025f * Kernel.fpsUnscaledSmoothDeltaTime);
        }
    }
}
