using SCKRM;
using SDJK.Map;
using SDJK.Replay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Ruleset.ReplayResult
{
    public sealed class ReplayResultUIDifficultyText : ReplayResultUIBase
    {
        [SerializeField, NotNull] ColorBand gradient;

        [SerializeField, NotNull] Image background;
        [SerializeField, NotNull] TMP_Text text;

        public override void Refresh(IRuleset ruleset, MapFile map, ReplayFile replay)
        {
            base.Refresh(ruleset, map, replay);

            background.color = gradient.Evaluate((float)(map.difficulty / 10d));
            text.text = map.difficulty.Round(2).ToString();
        }

        public override void ObjectReset()
        {
            base.ObjectReset();
            text.text = "";
        }
    }
}
