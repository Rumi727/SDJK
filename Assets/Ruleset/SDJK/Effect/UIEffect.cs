using SCKRM;
using SCKRM.Rhythm;
using SDJK.Ruleset.SDJK.Effect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public class UIEffect : SDJKEffect
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
