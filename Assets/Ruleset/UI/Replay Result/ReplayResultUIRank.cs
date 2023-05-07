using SCKRM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Ruleset.UI.ReplayResult
{
    public sealed class ReplayResultUIRank : ReplayResultUIBase
    {
        [SerializeField, NotNull] Graphic background;
        [SerializeField, NotNull] TMP_Text text;

        double rankProgressAnimation = 1;
        public override void RealUpdate(float lerpValue)
        {
            double realAccuracy = replay.rankProgresses.GetValue(double.MaxValue);
            rankProgressAnimation = rankProgressAnimation.Lerp(realAccuracy, lerpValue);
            if (rankProgressAnimation.Distance(realAccuracy) <= 0.001)
                rankProgressAnimation = realAccuracy;

            RankMetaData rank = ruleset.GetRank(rankProgressAnimation);
            background.color = rank.color;
            text.text = rank.name;
        }

        public override void ObjectReset()
        {
            base.ObjectReset();
            rankProgressAnimation = 1;

            background.color = Color.clear;
            text.text = "";
        }
    }
}
