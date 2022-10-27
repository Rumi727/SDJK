using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.Effect
{
    public abstract class SDJKEffect : global::SDJK.Effect.Effect
    {
        public new SDJKMapFile map => (SDJKMapFile)base.map;
    }
}
