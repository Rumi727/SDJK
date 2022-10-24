using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Effect
{
    public sealed class CameraEffect : Effect
    {
        public override void Refresh(bool force = false) { }

        void Update()
        {
            if (map == null)
            {
                transform.position = new Vector3(0, 0, -14);
                transform.eulerAngles = Vector3.zero;

                return;
            }

            transform.position = map.globalEffect.cameraPos.GetValue() + new Vector3(0, 0, (float)(-14 * map.globalEffect.cameraZoom.GetValue() + 14));
            transform.eulerAngles = map.globalEffect.cameraRotation.GetValue();
        }
    }
}
