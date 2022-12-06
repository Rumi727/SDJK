using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Effect
{
    public sealed class CameraEffect : Effect
    {
        public const float defaultDistance = 14;
        public override void Refresh(bool force = false) { }

        protected override void RealUpdate()
        {
            if (map == null)
            {
                transform.position = new Vector3(0, 0, -defaultDistance);
                transform.eulerAngles = Vector3.zero;

                return;
            }

            transform.position = map.globalEffect.cameraPos.GetValue() + new Vector3(0, 0, (float)(-defaultDistance * map.globalEffect.cameraZoom.GetValue() + defaultDistance));
            transform.eulerAngles = map.globalEffect.cameraRotation.GetValue();
        }
    }
}
