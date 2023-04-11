using SCKRM.Sound;
using SDJK.Ruleset;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Effect
{
    public sealed class EffectManager : MonoBehaviour
    {
        public IRuleset selectedRuleset { get; set; }
        public Map.MapPack selectedMapPack { get; set; }
        public Map.MapFile selectedMap { get; set; }

        public ISoundPlayer soundPlayer { get; set; }

        [SerializeField] List<Effect> _effects = new List<Effect>(); public List<Effect> effects => _effects;

        public void AllRefresh(bool force = false)
        {
            for (int i = 0; i < effects.Count; i++)
                effects[i].Refresh(force);
        }
    }
}
