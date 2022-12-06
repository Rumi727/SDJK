using SCKRM.Rhythm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Effect
{
    public abstract class Effect : MonoBehaviour
    {
        [SerializeField] EffectManager _effectManager; public virtual EffectManager effectManager { get => _effectManager; protected set => _effectManager = value; }

        public virtual Map.MapPack mapPack => effectManager.selectedMapPack;
        public virtual Map.MapFile map => effectManager.selectedMap;

        public abstract void Refresh(bool force = false);

        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            RealUpdate();
        }

        protected virtual void RealUpdate() { }
    }
}
