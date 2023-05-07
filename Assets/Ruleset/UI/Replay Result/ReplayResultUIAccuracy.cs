using SCKRM;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.UI.ReplayResult
{
    public sealed class ReplayResultUIAccuracy : ReplayResultUIBase
    {
        [SerializeField, NotNull] TMP_Text text;

        double accuracyAnimation = 1;
        public override void RealUpdate(float lerpValue)
        {
            accuracyAnimation = accuracyAnimation.Lerp(replay.accuracyAbses.GetValue(double.MaxValue), lerpValue);
            text.text = 100d.Lerp(0d, accuracyAnimation).Floor(2).ToString("0.##") + "%";
        }

        public override void ObjectReset()
        {
            base.ObjectReset();

            accuracyAnimation = 1;
            text.text = "";
        }
    }
}
