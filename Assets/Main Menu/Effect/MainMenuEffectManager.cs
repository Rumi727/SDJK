using SDJK.Effect;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.MainMenu
{
    public class MainMenuEffectManager : MonoBehaviour
    {
        [SerializeField] EffectManager effectManager;

        Map.MapFile lastMap;
        void Update()
        {
            if (MapManager.selectedMap != null && BGMManager.bgm != null && BGMManager.bgm.soundPlayer != null && lastMap != MapManager.selectedMap)
            {
                effectManager.selectedMapPack = MapManager.selectedMapPack;
                effectManager.selectedMap = MapManager.selectedMap;

                effectManager.soundPlayer = BGMManager.bgm.soundPlayer;

                effectManager.AllRefresh(false);
                lastMap = MapManager.selectedMap;
            }
        }
    }
}
