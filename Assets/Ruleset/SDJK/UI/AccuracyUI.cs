using SCKRM;
using SCKRM.Rhythm;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.UI
{
    public sealed class AccuracyUI : SDJKUI
    {
        [SerializeField, NotNull] TMP_Text text;
        [SerializeField] float lerpAniValue = 0.2f;
        [SerializeField] string suffix = "%";

        double value = 0;
        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            value = value.Lerp(judgementManager.accuracyAbs, lerpAniValue * RhythmManager.bpmFpsDeltaTime);
            text.text = 100d.Lerp(0d, value).Floor(2).ToString("0.##") + suffix;
        }
    }
}
