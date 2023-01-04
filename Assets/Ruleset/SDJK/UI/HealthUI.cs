using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Rhythm;
using SDJK.Ruleset.SDJK.Judgement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.UI
{
    public sealed class HealthUI : SDJKUI
    {
        [SerializeField] float lerpAniValue = 0.2f;
        [SerializeField] RectTransform valueImage;

        float value = 1;
        void Update()
        {
            if (!RhythmManager.isPlaying || SDJKJudgementManager.instance == null)
                return;

            value = value.Lerp((float)(SDJKJudgementManager.instance.health / SDJKJudgementManager.maxHealth), lerpAniValue * RhythmManager.bpmFpsDeltaTime);
            valueImage.anchorMax = new Vector2(1, value);
        }
    }
}
