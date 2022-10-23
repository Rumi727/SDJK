using SCKRM.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.Effect
{
    public sealed class EffectManager : MonoBehaviour
    {
        public Map.Map map { get; set; }
        public ISoundPlayer soundPlayer { get; set; }
    }
}
