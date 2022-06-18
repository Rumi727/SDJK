using SCKRM.Object;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK
{
    public class BackgroundVideoSetting : MonoBehaviour
    {
        public static BackgroundVideo backgroundVideo { get; private set; } = null;

        SDJKMap tempSDJKMap;
        void Update()
        {
            if (MapManager.selectedMap != null && tempSDJKMap != MapManager.selectedMap)
            {
                if (backgroundVideo != null)
                    backgroundVideo.PadeOut().Forget();

                backgroundVideo = (BackgroundVideo)ObjectPoolingSystem.ObjectCreate("background_video_setting.background_video", transform, false).monoBehaviour;
                tempSDJKMap = MapManager.selectedMap;
            }
        }
    }
}
