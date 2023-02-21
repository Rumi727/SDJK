using SCKRM;
using SCKRM.Rhythm;
using SDJK.Ruleset.SDJK.Judgement;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.UI
{
    public sealed class ScoreUI : SDJKUI
    {
        [SerializeField, NotNull] TMP_Text text;
        [SerializeField] float lerpAniValue = 0.2f;

        double value = 0;
        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            value = value.Lerp(judgementManager.score, lerpAniValue * RhythmManager.bpmFpsDeltaTime);
            text.text = value.Round().ToString();
        }
    }
}
