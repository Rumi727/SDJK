using SCKRM;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Ruleset.UI.ReplayResult
{
    public sealed class ReplayResultUIRankProgress : ReplayResultUIBase
    {
        [SerializeField, FieldNotNull] Image background;
        [SerializeField, FieldNotNull] Image color;

        double scoreAnimation = 1;
        public override void RealUpdate(float lerpValue)
        {
            scoreAnimation = scoreAnimation.Lerp(replay.rankProgresses.GetValue(double.MaxValue), lerpValue);
            float fillAmout = (float)(1 - scoreAnimation);

            background.fillAmount = 1 - fillAmout;
            color.fillAmount = fillAmout;
        }

        public override void ObjectReset()
        {
            base.ObjectReset();
            scoreAnimation = 1;

            background.fillAmount = 1;
            color.fillAmount = 0;
        }
    }
}
