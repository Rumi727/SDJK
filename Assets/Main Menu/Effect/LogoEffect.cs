using SCKRM;
using SCKRM.Rhythm;
using SCKRM.UI;
using SDJK.Effect;
using UnityEngine;

namespace SDJK.MainMenu
{
    public sealed class LogoEffect : UIEffect
    {
        public Vector2 pos;

        [SerializeField, NotNull] Camera mainCamera;
        [SerializeField, NotNull] RectTransform text;

        public override void Refresh(bool force = false) { }

        protected override void RealUpdate()
        {
            if (MainMenu.SaveData.logoEffectEnable)
            {
                if (map == null)
                {
                    transform.position = new Vector3(0, 0, -CameraEffect.defaultDistance);
                    transform.eulerAngles = Vector3.zero;

                    return;
                }

                Vector3 cameraPos = map.globalEffect.cameraPos.GetValue(RhythmManager.currentBeatScreen);
                float cameraRotation = -map.globalEffect.cameraRotation.GetValue(RhythmManager.currentBeatScreen).z;
                float cameraZoom = (float)map.globalEffect.cameraZoom.GetValue(RhythmManager.currentBeatScreen);
                float cameraZoomToZPos = (float)((cameraZoom * CameraEffect.defaultDistance) - CameraEffect.defaultDistance);

                float height = mainCamera.pixelHeight / canvas.transform.localScale.x;
                float screenY;

                if (mainCamera.orthographic)
                    screenY = mainCamera.orthographicSize * 2;
                else
                    screenY = Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2.0f * (-cameraPos.z + cameraZoomToZPos);


                float zoom = screenY / height;
                float posX = ((-cameraPos.x) * Mathf.Cos(cameraRotation * Mathf.Deg2Rad)) - ((-cameraPos.y) * Mathf.Sin(cameraRotation * Mathf.Deg2Rad));
                float posY = ((-cameraPos.x) * Mathf.Sin(cameraRotation * Mathf.Deg2Rad)) + ((-cameraPos.y) * Mathf.Cos(cameraRotation * Mathf.Deg2Rad));
                pos = new Vector2(posX, posY) / zoom;
                text.localEulerAngles = new Vector3(0, 0, cameraRotation);
            }
            else
            {
                pos = Vector2.zero;
                text.localEulerAngles = Vector3.zero;
            }
        }
    }
}
