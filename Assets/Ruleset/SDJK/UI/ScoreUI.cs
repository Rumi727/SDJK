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
        [SerializeField] float lerpAniValue = 0.125f;

        double value = 0;
        void Update()
        {
            if (!RhythmManager.isPlaying || SDJKJudgementManager.instance == null)
                return;

            value = value.Lerp(SDJKJudgementManager.instance.score, lerpAniValue * RhythmManager.bpmFpsDeltaTime);
            text.text = value.RoundToInt().ToString();
        }
    }
}
