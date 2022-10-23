using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK
{
    public sealed class CameraEffect : MonoBehaviour
    {
        void Update()
        {
            Map.Map map = MapManager.selectedMap;
            transform.position = map.globalEffect.cameraPos.GetValue() + new Vector3(0, 0, (float)(-14 * map.globalEffect.cameraZoom.GetValue() + 14));
            transform.eulerAngles = map.globalEffect.cameraRotation.GetValue();
        }
    }
}
