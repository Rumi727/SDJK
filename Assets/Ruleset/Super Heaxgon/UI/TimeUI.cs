using SCKRM;
using SCKRM.Rhythm;
using SDJK.Ruleset.SuperHexagon.Judgement;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.UI
{
    public sealed class TimeUI : SuperHexagonUIBase
    {
        [SerializeField, FieldNotNull] TMP_Text text;

        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            if (RhythmManager.time > 0)
            {
                int timeDecimalBase60 = (int)(RhythmManager.time.Repeat(1) * 60f);
                text.text = $"{(int)RhythmManager.time}<size={text.fontSize * 0.5}>:{timeDecimalBase60:00}</size>";
            }
            else
                text.text = $"0<size={text.fontSize * 0.5}>:00</size>";
        }
    }
}
