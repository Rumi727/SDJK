using SCKRM;
using SCKRM.Rhythm;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Ruleset.SuperHexagon.UI
{
    public sealed class HealthUI : SuperHexagonUIBase
    {
        [SerializeField] Field field;

        [SerializeField, NotNull] Image backgroundImage;
        [SerializeField, NotNull] Image valueImage;
        [SerializeField, NotNull] RectTransform valueRectTransform;

        [SerializeField] float lerpAniValue = 0.2f;

        float value = 1;
        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            backgroundImage.color = field.backgroundColor * new Color(0.25f, 0.25f, 0.25f);
            valueImage.color = field.mainColor;

            value = value.Lerp((float)(judgementManager.health / JudgementManagerBase.maxHealth), lerpAniValue * RhythmManager.bpmFpsDeltaTime);
            valueRectTransform.anchorMax = new Vector2(1, value);
        }
    }
}
