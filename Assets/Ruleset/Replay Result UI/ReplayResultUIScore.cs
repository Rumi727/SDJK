using SCKRM;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.ReplayResult
{
    public sealed class ReplayResultUIScore : ReplayResultUIBase
    {
        [SerializeField, NotNull] TMP_Text text;

        double scoreAnimation = 0;
        public override void RealUpdate(float lerpValue)
        {
            scoreAnimation = scoreAnimation.Lerp(replay.scores.GetValue(double.MaxValue), lerpValue);
            text.text = scoreAnimation.RoundToInt().ToString();
        }

        public override void Remove()
        {
            scoreAnimation = 0;
            text.text = "";
        }
    }
}
