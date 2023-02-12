using SCKRM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Ruleset.ReplayResult
{
    public sealed class ReplayResultUIRank : ReplayResultUIBase
    {
        [SerializeField, NotNull] Graphic background;
        [SerializeField, NotNull] TMP_Text text;

        double accuracyAnimation = 1;
        public override void RealUpdate(float lerpValue)
        {
            double realAccuracy = replay.accuracys.GetValue(double.MaxValue);
            accuracyAnimation = accuracyAnimation.Lerp(realAccuracy, lerpValue);
            if (accuracyAnimation.Distance(realAccuracy) <= 0.001)
                accuracyAnimation = realAccuracy;

            RankMetaData rank = ruleset.GetRank(accuracyAnimation);
            background.color = rank.color;
            text.text = rank.name;
        }

        public override void ObjectReset()
        {
            base.ObjectReset();
            accuracyAnimation = 1;

            background.color = Color.clear;
            text.text = "";
        }
    }
}
