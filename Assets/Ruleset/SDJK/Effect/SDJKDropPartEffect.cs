using SDJK.Effect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.Effect
{
    public class SDJKDropPartEffect : DropPartEffect
    {
        [SerializeField] Bar _bar; public Bar bar => _bar;

        protected override void RealUpdate()
        {
            if (effectManager == null)
                effectManager = bar.effectManager;

            base.RealUpdate();
        }
    }
}
