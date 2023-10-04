using SCKRM;
using SCKRM.Input;
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

        double lerpValue = 0;
        bool invokeLock = false;
        double startDelay = 0;
        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            if (RhythmManager.offset > RhythmManager.startDelay)
                startDelay = RhythmManager.offset;
            else
                startDelay = RhythmManager.startDelay;

            invokeLock = true;
            lerpValue = lerpValue.Lerp(0, lerpAniValue * Kernel.fpsUnscaledSmoothDeltaTime);
            
            slider.value = (float)(startDelay + RhythmManager.time + lerpValue);
            slider.maxValue = (float)(startDelay + RhythmManager.length);
            
            invokeLock = false;

            timeText.text = RhythmManager.time.ToTime(false, true);
            timeRemainingText.text = (RhythmManager.length - RhythmManager.time).ToTime(false, true);

            if (judgementManager.sdjkManager.isReplay)
            {
                if (InputManager.TryGetKey("map_manager.previous_music"))
                    TimeChange(RhythmManager.time - 10);
                else if (InputManager.TryGetKey("map_manager.next_music"))
                    TimeChange(RhythmManager.time + 10);

                if (InputManager.TryGetKey("map_manager.pause_music"))
                    RhythmManager.isPaused = !RhythmManager.isPaused;
            }
        }

        public void OnValueChanged(float value)
        {
            if (invokeLock || !judgementManager.sdjkManager.isReplay)
                return;

            TimeChange(value - startDelay);
        }

        void TimeChange(double time)
        {
            lerpValue += RhythmManager.time - time;
            RhythmManager.time = time;
        }
    }
}
