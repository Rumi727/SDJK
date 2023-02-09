using SCKRM;
using SDJK.Map;
using SDJK.Replay;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.ReplayResult
{
    public sealed class ReplayResultUIDifficultyLabel : ReplayResultUIBase
    {
        [SerializeField, NotNull] TMP_Text text;

        public override void Refresh(IRuleset ruleset, MapFile map, ReplayFile replay)
        {
            base.Refresh(ruleset, map, replay);
            text.text = map.info.difficultyLabel;
        }

        public override void ObjectReset() => text.text = "";
    }
}
