using SCKRM;
using SCKRM.Rhythm;
using SDJK.Ruleset.SuperHexagon.Judgement;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.UI
{
    public sealed class ScoreUI : SuperHexagonUI
    {
        [SerializeField, NotNull] TMP_Text text;
        [SerializeField] float lerpAniValue = 0.2f;

        double value = 0;
        void Update()
        {
            if (!RhythmManager.isPlaying || SuperHexagonJudgementManager.instance == null)
                return;

            value = value.Lerp(SuperHexagonJudgementManager.instance.score, lerpAniValue * RhythmManager.bpmFpsDeltaTime);
            text.text = value.Round().ToString();
        }
    }
}
