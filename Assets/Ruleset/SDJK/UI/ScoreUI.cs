using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Rhythm;
using SDJK.Ruleset.SDJK.Judgement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.UI
{
    public sealed class ScoreUI : SDJKUI
    {
        [SerializeField] TMP_Text text;

        protected override void JudgementAction(double disSecond, bool isMiss, JudgementMetaData metaData) => text.text = SDJKJudgementManager.instance.score.RoundToInt().ToString();
    }
}
