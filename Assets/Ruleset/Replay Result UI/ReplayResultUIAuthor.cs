using SCKRM;
using SDJK.Map;
using SDJK.Replay;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.ResultScreen
{
    public sealed class ReplayResultUIAuthor : ReplayResultUIBase
    {
        [SerializeField, NotNull] TMP_Text text;

        public override void Refresh(IRuleset ruleset, MapFile map, ReplayFile replay)
        {
            base.Refresh(ruleset, map, replay);
            text.text = map.info.author;
        }

        public override void Remove() => text.text = "";
    }
}
