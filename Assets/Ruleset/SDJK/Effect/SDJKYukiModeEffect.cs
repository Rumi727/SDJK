using SDJK.Effect;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.Effect
{
    public class SDJKYukiModeEffect : YukiModeEffect
    {
        public override EffectManager effectManager => bar.effectManager;
        [SerializeField] Bar _bar; public Bar bar => _bar;
    }
}
