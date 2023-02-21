using SCKRM;
using SCKRM.UI;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.UI
{
    public sealed class RankUI : SuperHexagonUIBase
    {
        [SerializeField, NotNull] SuperHexagonManager manager;
        [SerializeField, NotNull] TMP_Text text;

        protected override void JudgementAction(bool isMiss)
        {
            RankMetaData rank = manager.ruleset.GetRank(judgementManager.accuracy);

            text.text = rank.name;
            text.color = rank.color;
        }
    }
}
