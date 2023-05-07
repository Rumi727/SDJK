using SCKRM;
using SDJK.Map;
using SDJK.Replay;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.UI.ReplayResult
{
    public sealed class ReplayResultUIGameOver : ReplayResultUIBase
    {
        public override void Refresh(IRuleset ruleset, MapFile map, ReplayFile replay)
        {
            base.Refresh(ruleset, map, replay);
            gameObject.SetActive(replay.isGameOver);
        }

        public override void ObjectReset()
        {
            base.ObjectReset();
            gameObject.SetActive(false);
        }
    }
}
