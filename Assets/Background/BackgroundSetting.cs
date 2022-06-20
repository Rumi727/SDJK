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

        SDJKMapPack lastSDJKMapPack;
        SDJKMap lastSDJKMap;
        string lastBackgroundFile;
        string lastBackgroundNightFile;
        void Update()
        {
            if (MapManager.selectedMap != null)
            {
                bool isBackgroundChanged = (lastBackgroundFile != MapManager.selectedMapInfo.backgroundFile || lastBackgroundNightFile != MapManager.selectedMapInfo.backgroundNightFile);
                if (lastSDJKMapPack != MapManager.selectedMapPack || (lastSDJKMap != MapManager.selectedMap && isBackgroundChanged))
                {
                    if (background != null)
                        background.PadeOut().Forget();

                    background = (Background)ObjectPoolingSystem.ObjectCreate("background_setting.background", transform, false).monoBehaviour;

                    lastSDJKMapPack = MapManager.selectedMapPack;
                    lastSDJKMap = MapManager.selectedMap;
                    lastBackgroundFile = MapManager.selectedMapInfo.backgroundFile;
                    lastBackgroundNightFile = MapManager.selectedMapInfo.backgroundNightFile;
                }
            }
        }
    }
}
