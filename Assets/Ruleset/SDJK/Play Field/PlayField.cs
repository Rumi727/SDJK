using SCKRM;
using SCKRM.Object;
using SCKRM.Rhythm;
using SDJK.Effect;
using SDJK.Map.Ruleset.SDJK.Map;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public sealed class PlayField : ObjectPooling
    {
        [SerializeField] Transform _bars; public Transform bars => _bars;

        public FieldEffectFile fieldEffectFile { get; private set; }
        public int fieldIndex { get; private set; } = 0;
        public double fieldHeight { get; private set; } = 16;

        public EffectManager effectManager => SDJKManager.instance.effectManager;
        public SDJKMapFile map => (SDJKMapFile)effectManager.selectedMap;

        void Update() => fieldHeight = fieldEffectFile.height.GetValue(RhythmManager.currentBeatScreen);

        List<Bar> createdBars = new List<Bar>();
        public void Refresh(int fieldIndex)
        {
            this.fieldIndex = fieldIndex;

            fieldEffectFile = map.effect.fieldEffect[fieldIndex];

            BarAllRemove();
            for (int i = 0; i < fieldEffectFile.barEffect.Count.Min(map.notes.Count); i++)
            {
                Bar bar = (Bar)ObjectPoolingSystem.ObjectCreate("ruleset.sdjk.play_field.bar", bars).monoBehaviour;
                bar.Refresh(this, i);

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
