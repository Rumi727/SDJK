using SCKRM;
using SCKRM.Rhythm;
using SDJK.Ruleset.SuperHexagon.Judgement;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.UI
{
    public sealed class HealthUI : SuperHexagonUIBase
    {
        [SerializeField] float lerpAniValue = 0.2f;
        [SerializeField, NotNull] RectTransform valueImage;

        float value = 1;
        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            value = value.Lerp((float)(judgementManager.health / SuperHexagonJudgementManager.maxHealth), lerpAniValue * RhythmManager.bpmFpsDeltaTime);
            valueImage.anchorMax = new Vector2(1, value);
        }
    }
}
