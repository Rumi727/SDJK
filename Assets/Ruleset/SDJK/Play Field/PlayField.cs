using SCKRM.Object;
using SDJK.Effect;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public sealed class PlayField : ObjectPooling
    {
        [SerializeField] Transform _bars; public Transform bars => _bars;

        public EffectManager effectManager { get; private set; }
        public SDJKMapFile map => (SDJKMapFile)effectManager.selectedMap;

        List<Bar> createdBars = new List<Bar>();
        public void Refresh(EffectManager effectManager)
        {
            this.effectManager = effectManager;

            BarAllRemove();
            for (int i = 0; i < map.notes.Count; i++)
            {
                Bar bar = (Bar)ObjectPoolingSystem.ObjectCreate("ruleset.sdjk.play_field.bar", bars).monoBehaviour;
                bar.Refresh(effectManager, i);

                createdBars.Add(bar);
            }
        }

        void BarAllRemove()
        {
            for (int i = 0; i < createdBars.Count; i++)
                createdBars[i].Remove();

            createdBars.Clear();
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            BarAllRemove();
            return true;
        }
    }
}
