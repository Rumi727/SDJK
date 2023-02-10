using SDJK.Effect;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.Effect
{
    public class SDJKYukiModeEffect : YukiModeEffect
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
