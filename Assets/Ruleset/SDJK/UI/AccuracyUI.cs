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
    public sealed class AccuracyUI : SDJKUI
    {
        [SerializeField] TMP_Text text;
        [SerializeField] float lerpAniValue = 0.2f;
        [SerializeField] string suffix = "%";

        double value = 0;
        void Update()
        {
            if (!RhythmManager.isPlaying || SDJKJudgementManager.instance == null)
                return;

            value = value.Lerp(SDJKJudgementManager.instance.accuracyAbs, lerpAniValue * RhythmManager.bpmFpsDeltaTime);
            text.text = 100d.Lerp(0d, value).Round(2).ToString() + suffix;
        }
    }
}
