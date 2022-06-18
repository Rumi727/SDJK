using SCKRM.Object;
using SCKRM.Rhythm;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK
{
    public sealed class BackgroundSetting : MonoBehaviour
    {
        public static Background background { get; private set; } = null;

        SDJKMap tempSDJKMap;
        void Update()
        {
            if (MapManager.selectedMap != null && tempSDJKMap != MapManager.selectedMap)
            {
                if (background != null)
                    background.PadeOut().Forget();

                background = (Background)ObjectPoolingSystem.ObjectCreate("background_setting.background", transform, false).monoBehaviour;
                tempSDJKMap = MapManager.selectedMap;
            }
        }
    }
}
