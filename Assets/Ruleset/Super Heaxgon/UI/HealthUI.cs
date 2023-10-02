using SCKRM;
using SCKRM.Rhythm;
using SDJK.Mode;
using SDJK.Mode.Difficulty;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Ruleset.SuperHexagon.UI
{
    public sealed class HealthUI : SuperHexagonUIBase
    {
        [SerializeField] Field field;

        [SerializeField, FieldNotNull] Image backgroundImage;
        [SerializeField, FieldNotNull] Image valueImage;
        [SerializeField, FieldNotNull] RectTransform valueRectTransform;

        [SerializeField] float lerpAniValue = 0.2f;

        float value = 1;
        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            bool active = judgementManager.manager.modes.FindMode<SuddenDeathModeBase>() == null;
            if (active != gameObject.activeSelf)
                gameObject.SetActive(active);
            
            backgroundImage.color = field.backgroundColor * new Color(0.25f, 0.25f, 0.25f);
            valueImage.color = field.mainColor;

            value = value.Lerp((float)(judgementManager.health / JudgementManagerBase.maxHealth), lerpAniValue * RhythmManager.bpmFpsDeltaTime);
            valueRectTransform.anchorMax = new Vector2(1, value);
        }
    }
}
