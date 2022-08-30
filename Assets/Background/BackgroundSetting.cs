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

        Map.Map lastSDJKMap;
        void Update()
        {
            if (MapManager.selectedMap != null)
            {
                if (lastSDJKMap != MapManager.selectedMap)
                {
                    if (background != null && !background.isRemoved)
                        background.padeOut = true;

                    background = (Background)ObjectPoolingSystem.ObjectCreate(prefab, transform, false).monoBehaviour;
                    lastSDJKMap = MapManager.selectedMap;
                }
            }
        }
    }
}
