using SCKRM;
using SCKRM.Rhythm;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Ruleset.SDJK.UI
{
    public sealed class TimeSliderUI : SDJKUI
    {
        [SerializeField, FieldNotNull] Slider slider;

        [SerializeField, FieldNotNull] TMP_Text timeText;
        [SerializeField, FieldNotNull] TMP_Text timeRemainingText;

        [SerializeField] float lerpAniValue = 0.2f;

        double value = 0;
        bool invokeLock = false;
        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            invokeLock = true;
            value = value.Lerp(RhythmManager.time, lerpAniValue * RhythmManager.bpmFpsDeltaTime);

            slider.value = (float)value;
            slider.maxValue = (float)RhythmManager.length;
            
            invokeLock = false;

            timeText.text = RhythmManager.time.ToTime(false, true);
            timeRemainingText.text = (RhythmManager.length - RhythmManager.time).ToTime(false, true);
        }

        public void OnValueChanged(float value)
        {
            if (invokeLock || !judgementManager.sdjkManager.isReplay)
                return;

            RhythmManager.time = value;
        }
    }
}
