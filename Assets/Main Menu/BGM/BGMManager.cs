using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Object;
using SCKRM.Resource;
using SCKRM.Rhythm;
using SCKRM.Sound;
using SCKRM.Splash;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.MainMenu
{
    public sealed class BGMManager : Manager<BGMManager>
    {
        public static BGM bgm { get; private set; }



        void Awake() => SingletonCheck(this);

        void OnEnable() => ResourceManager.audioResetEnd += Refresh;
        void OnDisable () => ResourceManager.audioResetEnd -= Refresh;



        MapPack tempSDJKMapPack;
        Map.Map tempSDJKMap;
        Map.Map tempSDJKMap2;
        string tempSongFile = "";
        void Update()
        {
            if (!InitialLoadManager.isSceneMoveEnd)
                return;

            if (MapManager.selectedMap != null)
            {
                if (tempSDJKMapPack != MapManager.selectedMapPack || (tempSDJKMap != MapManager.selectedMap && tempSongFile != MapManager.selectedMapInfo.songFile))
                    Refresh();

                if ((tempSDJKMap2 != MapManager.selectedMap || !RhythmManager.isPlaying) && bgm != null && bgm.soundPlayer != null)
                {
                    RhythmManager.Stop();
                    RhythmManager.Play(MapManager.selectedMapEffect.bpm, MapManager.selectedMapInfo.songOffset, MapManager.selectedMapEffect.dropPart, bgm.soundPlayer);

                    tempSDJKMap2 = MapManager.selectedMap;
                }
            }
        }

        void Refresh()
        {
            if (bgm != null && !bgm.isRemoved)
                bgm.padeOut = true;

            bgm = (BGM)ObjectPoolingSystem.ObjectCreate("bgm_manager.bgm", transform, false).monoBehaviour;

            tempSDJKMapPack = MapManager.selectedMapPack;
            tempSDJKMap = MapManager.selectedMap;
            tempSongFile = MapManager.selectedMapInfo.songFile;
        }
    }
}
