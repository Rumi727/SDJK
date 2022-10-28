using SCKRM.Object;
using SCKRM.Rhythm;
using SDJK.Effect;
using SDJK.Ruleset.SDJK.Map;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public sealed class PlayField : ObjectPooling
    {
        [SerializeField] Transform _bars; public Transform bars => _bars;

        public int fieldIndex { get; private set; } = 0;
        public double fieldHeight { get; private set; } = 16;

        public EffectManager effectManager { get; private set; }
        public SDJKMapFile map => (SDJKMapFile)effectManager.selectedMap;

        void Update()
        {
            if (map.effect.fieldHeight.Count > fieldIndex)
                fieldHeight = map.effect.fieldHeight[fieldIndex].GetValue(RhythmManager.currentBeatScreen);

            if (map.effect.fieldPos.Count > fieldIndex)
                transform.localPosition = map.effect.fieldPos[fieldIndex].GetValue(RhythmManager.currentBeatScreen);

            if (map.effect.fieldRotation.Count > fieldIndex)
                transform.localEulerAngles = map.effect.fieldRotation[fieldIndex].GetValue(RhythmManager.currentBeatScreen);


            Camera camera = Camera.main;
            float screenY;
            if (camera.orthographic)
                screenY = camera.orthographicSize * 2;
            else
                screenY = Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2.0f * CameraEffect.defaultDistance;

            float scale = (float)(screenY / fieldHeight);
            transform.localScale = new Vector3(scale, scale, scale);
        }

        List<Bar> createdBars = new List<Bar>();
        public void Refresh(int fieldIndex, EffectManager effectManager)
        {
            this.fieldIndex = fieldIndex;
            this.effectManager = effectManager;

            BarAllRemove();
            for (int i = 0; i < map.notes.Count; i++)
            {
                Bar bar = (Bar)ObjectPoolingSystem.ObjectCreate("ruleset.sdjk.play_field.bar", bars).monoBehaviour;
                bar.Refresh(this, effectManager, i);

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
