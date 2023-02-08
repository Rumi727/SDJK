using SCKRM;
using SCKRM.Rhythm;
using SDJK.Ruleset.SDJK.Judgement;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.UI
{
    public sealed class HealthUI : SDJKUI
    {
        [SerializeField] float lerpAniValue = 0.2f;
        [SerializeField, NotNull] RectTransform valueImage;

        float value = 1;
        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            value = value.Lerp((float)(judgementManager.health / SDJKJudgementManager.maxHealth), lerpAniValue * RhythmManager.bpmFpsDeltaTime);
            valueImage.anchorMax = new Vector2(1, value);
        }
    }
}
