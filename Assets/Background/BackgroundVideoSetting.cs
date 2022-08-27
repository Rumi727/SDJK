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

        MapPack lastSDJKMapPack;
        Map.Map lastSDJKMap;
        string lastVideoBackgroundFile;
        string lastVideoBackgroundNightFile;
        void Update()
        {
            if (MapManager.selectedMapInfo == null)
                return;

            bool isVideoBackgroundChanged = (lastVideoBackgroundFile != MapManager.selectedMapInfo.videoBackgroundFile || lastVideoBackgroundNightFile != MapManager.selectedMapInfo.videoBackgroundNightFile);
            if (lastSDJKMapPack != MapManager.selectedMapPack || (lastSDJKMap != MapManager.selectedMap && isVideoBackgroundChanged))
            {
                if (backgroundVideo != null)
                    backgroundVideo.PadeOut().Forget();

                backgroundVideo = (BackgroundVideo)ObjectPoolingSystem.ObjectCreate("background_video_setting.background_video", transform, false).monoBehaviour;

                lastSDJKMapPack = MapManager.selectedMapPack;
                lastSDJKMap = MapManager.selectedMap;
                lastVideoBackgroundFile = MapManager.selectedMapInfo.videoBackgroundFile;
                lastVideoBackgroundNightFile = MapManager.selectedMapInfo.videoBackgroundNightFile;
            }
        }
    }
}
