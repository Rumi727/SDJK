using SCKRM;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.ReplayResult
{
    public sealed class ReplayResultUIAccuracy : ReplayResultUIBase
    {
        [SerializeField, NotNull] TMP_Text text;

        double accuracyAnimation = 0;
        public override void RealUpdate(float lerpValue)
        {
            accuracyAnimation = accuracyAnimation.Lerp(replay.accuracys.GetValue(double.MaxValue), lerpValue);
            text.text = accuracyAnimation.Round(2).ToString() + "%";
        }

        public override void ObjectReset()
        {
            accuracyAnimation = 0;
            text.text = "";
        }
    }
}
