using SCKRM.Rhythm;
using SCKRM;
using SDJK.Ruleset.SDJK.Judgement;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public sealed class RankUI : SDJKUI
    {
        [SerializeField] SDJKManager manager;
        [SerializeField] TMP_Text text;

        protected override void JudgementAction(double disSecond, bool isMiss, double accuracy, JudgementMetaData metaData) => text.text = manager.ruleset.GetRank(accuracy.Abs()).name;
    }
}
