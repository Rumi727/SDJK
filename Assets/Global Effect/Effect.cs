using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Effect
{
    public abstract class Effect : MonoBehaviour
    {
        [SerializeField] EffectManager _effectManager; public virtual EffectManager effectManager { get => _effectManager; set => _effectManager = value; }

        public virtual Map.MapPack mapPack => effectManager.selectedMapPack;
        public virtual Map.MapFile map => effectManager.selectedMap;

        public abstract void Refresh(bool force = false);
    }
}
