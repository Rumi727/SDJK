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
        public Background background { get; private set; } = null;
        public string prefab { get => _prefab; set => _prefab = value; } [SerializeField] string _prefab = "background_setting.background";

        MapPack lastSDJKMapPack;
        Map.Map lastSDJKMap;
        string lastBackgroundFile;
        string lastBackgroundNightFile;
        void Update()
        {
            if (MapManager.selectedMap != null)
            {
                bool isBackgroundChanged = (lastBackgroundFile != MapManager.selectedMapInfo.backgroundFile || lastBackgroundNightFile != MapManager.selectedMapInfo.backgroundNightFile);
                if (lastSDJKMapPack != MapManager.selectedMapPack || (lastSDJKMap != MapManager.selectedMap && isBackgroundChanged))
                {
                    if (background != null && !background.isRemoved)
                        background.PadeOut().Forget();

                    background = (Background)ObjectPoolingSystem.ObjectCreate(prefab, transform, false).monoBehaviour;

                    lastSDJKMapPack = MapManager.selectedMapPack;
                    lastSDJKMap = MapManager.selectedMap;
                    lastBackgroundFile = MapManager.selectedMapInfo.backgroundFile;
                    lastBackgroundNightFile = MapManager.selectedMapInfo.backgroundNightFile;
                }
            }
        }
    }
}
