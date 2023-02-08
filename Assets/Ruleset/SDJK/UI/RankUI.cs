using SCKRM;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public sealed class RankUI : SDJKUI
    {
        [SerializeField, NotNull] SDJKManager manager;
        [SerializeField, NotNull] TMP_Text text;

        protected override void JudgementAction(double disSecond, bool isMiss, double accuracy, JudgementMetaData metaData)
        {
            RankMetaData rank = manager.ruleset.GetRank(judgementManager.accuracyAbs);

            text.text = rank.name;
            text.color = rank.color;
        }
    }
}
