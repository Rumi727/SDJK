using SCKRM;
using SCKRM.Rhythm;
using SDJK.Effect;
using UnityEngine;

namespace SDJK.MainMenu
{
    public sealed class LogoEffect : UIEffect
    {
        public Vector2 pos;

        [SerializeField, NotNull] Camera mainCamera;
        [SerializeField, NotNull] RectTransform text;
        [SerializeField, NotNull] RectTransform visualizer;

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
                cameraPos.x = -cameraPos.x;
                cameraPos.y = -cameraPos.y;

                float cameraRotation = -map.globalEffect.cameraRotation.GetValue(RhythmManager.currentBeatScreen).z;

                Rect lastRect = mainCamera.rect;
                mainCamera.rect = new Rect(0, 0, 1, 1);

                pos = (mainCamera.WorldToScreenPoint(Vector3.zero) * (canvas.pixelRect.height / ScreenManager.height));
                pos -= new Vector2(canvas.pixelRect.width * 0.5f, canvas.pixelRect.height * 0.5f);

                mainCamera.rect = lastRect;

                text.localEulerAngles = new Vector3(0, 0, cameraRotation);
                visualizer.localEulerAngles = new Vector3(0, 0, cameraRotation);
            }
            else
            {
                pos = Vector2.zero;
                text.localEulerAngles = Vector3.zero;
                visualizer.localEulerAngles = Vector3.zero;
            }
        }
    }
}
