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
        [SerializeField] RectTransform valueImage;

        void Update()
        {
            if (!RhythmManager.isPlaying || SDJKJudgementManager.instance == null)
                return;

            valueImage.anchorMax = new Vector2(1, (float)(SDJKJudgementManager.instance.health / SDJKJudgementManager.maxHealth));
        }

        protected override void JudgementAction(double disSecond, bool isMiss, JudgementMetaData metaData) { }
    }
}
