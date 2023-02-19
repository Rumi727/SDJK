using SCKRM.Rhythm;
using SDJK.Ruleset.SuperHexagon.Effect;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon
{
    public class UIEffect : SuperHexagonEffect
    {
        [SerializeField] Canvas canvas;

        protected override void RealUpdate()
        {
            if (map == null)
                return;

            canvas.scaleFactor = (float)map.globalEffect.uiSize.GetValue(RhythmManager.currentBeatScreen) * (canvas.pixelRect.height / 720f);
        }
    }
}
