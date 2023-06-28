using SCKRM;
using SCKRM.Rhythm;
using UnityEngine;

using Random = System.Random;

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

            //Camer Shake
            Vector3 drain = map.globalEffect.cameraShakeDrain.GetValue(RhythmManager.currentBeatScreen);
            {
                double delay = map.globalEffect.cameraShakeDelay.GetValue(RhythmManager.currentBeatSound);
                double time = RhythmManager.time - RhythmManager.time.Repeat(delay);

                Random random = new Random((int)(map.info.randomSeed * time * 5387195).Repeat(int.MaxValue));
                random.NextDouble();

                float x = (float)random.NextDouble() * drain.x;
                float y = (float)random.NextDouble() * drain.y;
                float z = (float)random.NextDouble() * drain.z;

                Vector3 offset = (Vector3)map.globalEffect.cameraShakeOffset.GetValue(RhythmManager.currentBeatScreen);
                transform.position += new Vector3(x, y, z) - new Vector3(drain.x * offset.x, drain.y * offset.y, drain.z * offset.z);
            }
        }
    }
}
