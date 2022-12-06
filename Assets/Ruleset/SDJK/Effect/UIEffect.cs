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

        public override void Refresh(bool force = false) { }

        void Update()
        {
            if (map == null)
                return;

            canvas.scaleFactor = (float)map.globalEffect.uiSize.GetValue(RhythmManager.currentBeatScreen) * (ScreenManager.height / 720f);
        }
    }
}
