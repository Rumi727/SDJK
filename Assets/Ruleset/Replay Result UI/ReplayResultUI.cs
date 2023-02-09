using SCKRM;
using SCKRM.Object;
using SDJK.Map;
using SDJK.Replay;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset
{
    public sealed class ReplayResultUI : ObjectPoolingBase
    {
        [SerializeField] TMP_Text ranking;
        [SerializeField] List<ReplayResultUIBase> replayResultUIBases = new List<ReplayResultUIBase>();

        public void Refresh(IRuleset ruleset, MapFile map, ReplayFile replay, int ranking)
        {
            for (int i = 0; i < replayResultUIBases.Count; i++)
                replayResultUIBases[i].Refresh(ruleset, map, replay);

            if (this.ranking != null)
                this.ranking.text = ranking.ToString();
        }

        public void ObjectReset()
        {
            for (int i = 0; i < replayResultUIBases.Count; i++)
                replayResultUIBases[i].ObjectReset();
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            ObjectReset();
            return true;
        }
    }
}
