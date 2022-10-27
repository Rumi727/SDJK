using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Effect
{
    public abstract class Effect : MonoBehaviour
    {
        [SerializeField] EffectManager _effectManager; public EffectManager effectManager { get => _effectManager; set => _effectManager = value; }

        public Map.MapPack mapPack => effectManager.selectedMapPack;
        public Map.Map map => effectManager.selectedMap;

        public abstract void Refresh(bool force = false);
    }
}
