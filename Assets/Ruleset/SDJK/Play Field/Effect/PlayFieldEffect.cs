using SCKRM.Rhythm;
using SDJK.Effect;
using SDJK.Ruleset.SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.Effect
{
    public class PlayFieldEffect : SDJKEffect
    {
        [SerializeField] PlayField playField;
        FieldEffectFile fieldEffectFile => playField.fieldEffectFile;

        public override void Refresh(bool force = false) { }

        protected override void RealUpdate()
        {
            if (effectManager == null)
                effectManager = playField.effectManager;

            PosUpdate();
            SizeUpdate();
        }

        void PosUpdate()
        {
            transform.localPosition = fieldEffectFile.pos.GetValue(RhythmManager.currentBeatScreen);
            transform.localEulerAngles = fieldEffectFile.rotation.GetValue(RhythmManager.currentBeatScreen);
        }

        void SizeUpdate()
        {
            Camera camera = Camera.main;
            float screenY;
            if (camera.orthographic)
                screenY = camera.orthographicSize * 2;
            else
                screenY = Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2.0f * CameraEffect.defaultDistance;

            float scale = (float)(screenY / playField.fieldHeight);
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
