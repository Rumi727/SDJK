using SCKRM;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.ReplayResult
{
    public sealed class ReplayResultUIMaxCombo : ReplayResultUIBase
    {
        [SerializeField, NotNull] TMP_Text text;

        double maxComboAnimation = 0;
        public override void RealUpdate(float lerpValue)
        {
            maxComboAnimation = maxComboAnimation.Lerp(replay.maxCombo.GetValue(double.MaxValue), lerpValue);
            text.text = maxComboAnimation.RoundToInt().ToString() + " / " + replay.mapMaxCombo;
        }

        public override void ObjectReset()
        {
            base.ObjectReset();

            maxComboAnimation = 0;
            text.text = "";
        }
    }
}
