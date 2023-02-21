using SCKRM;
using SCKRM.Rhythm;
using SDJK.Ruleset.SuperHexagon.Judgement;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Ruleset.SuperHexagon.UI
{
    public sealed class RankSliderUI : SuperHexagonUIBase
    {
        [SerializeField, NotNull] SuperHexagonManager manager;
        [SerializeField, NotNull] Image fill;

        double start = 0;
        double end = 1;
        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            fill.fillAmount = (float)end.InverseLerp(start, judgementManager.rankProgress);
        }

        protected override void JudgementAction(bool isMiss)
        {
            RankMetaData rank = manager.ruleset.GetRank(judgementManager.rankProgress, out int index);
            if (index > 0)
                start = manager.ruleset.rankMetaDatas[index - 1].size;
            else
                start = 0;

            end = rank.size;
        }
    }
}
