using SCKRM;
using SCKRM.Rhythm;
using UnityEngine;

namespace SDJK.Effect
{
    public sealed class CameraEffect : Effect
    {
        public const float defaultDistance = 14;
        public const float defaultOrthographicSize = 8;

        Camera _targetCamera; public Camera targetCamera => _targetCamera = this.GetComponentFieldSave(_targetCamera);

        protected override void RealUpdate()
        {
            if (map == null)
            {
                transform.position = new Vector3(0, 0, -defaultDistance);
                transform.eulerAngles = Vector3.zero;

                return;
            }

            transform.position = map.globalEffect.cameraPos.GetValue(RhythmManager.currentBeatScreen);
            if (targetCamera.orthographic)
                targetCamera.orthographicSize = (float)(defaultOrthographicSize * map.globalEffect.cameraZoom.GetValue(RhythmManager.currentBeatScreen));
            else
                transform.position += new Vector3(0, 0, (float)(-defaultDistance * map.globalEffect.cameraZoom.GetValue(RhythmManager.currentBeatScreen) + defaultDistance));

            transform.eulerAngles = map.globalEffect.cameraRotation.GetValue(RhythmManager.currentBeatScreen);
        }
    }
}
