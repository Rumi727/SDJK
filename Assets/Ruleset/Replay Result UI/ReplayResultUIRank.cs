using SCKRM;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.ResultScreen
{
    public sealed class ReplayResultUIRank : ReplayResultUIBase
    {
        [SerializeField, NotNull] TMP_Text text;

        double accuracyAnimation = 0;
        public override void RealUpdate(float lerpValue)
        {
            accuracyAnimation = accuracyAnimation.Lerp(replay.accuracys.GetValue(double.MaxValue), lerpValue);
            text.text = ruleset.GetRank(accuracyAnimation).name;
        }

        public override void Remove()
        {
            accuracyAnimation = 0;
            text.text = "";
        }
    }
}
