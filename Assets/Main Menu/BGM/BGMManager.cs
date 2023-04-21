using SCKRM;
using SCKRM.Object;
using SCKRM.Resource;
using SCKRM.Rhythm;
using SDJK.Map;

namespace SDJK.MainMenu
{
    public sealed class BGMManager : ManagerBase<BGMManager>
    {
        public static BGM bgm { get; private set; }



        void Awake() => SingletonCheck(this);

        void OnEnable() => ResourceManager.audioResetEnd += Refresh;
        void OnDisable () => ResourceManager.audioResetEnd -= Refresh;



        MapPack tempSDJKMapPack;
        MapFile tempSDJKMap;
        string tempSongFile = "";
        void Update()
        {
            if (!InitialLoadManager.isSceneMoveEnd)
                return;

            if (MapManager.selectedMap != null)
            {
                if (tempSDJKMapPack != MapManager.selectedMapPack || (tempSDJKMap != MapManager.selectedMap && tempSongFile != MapManager.selectedMapInfo.songFile))
                    Refresh();
            }
        }

        void Refresh()
        {
            double lastTime = 0;
            if (bgm != null && !bgm.isRemoved)
            {
                bgm.padeOut = true;
                lastTime = RhythmManager.time;
            }

            RhythmManager.Stop();
            RhythmManager.Play(MapManager.selectedMapEffect.bpm, MapManager.selectedMapInfo.songOffset, MapManager.selectedMapEffect.yukiMode);

            bgm = (BGM)ObjectPoolingSystem.ObjectCreate("bgm_manager.bgm", transform, false).monoBehaviour;
            bgm.Refresh(tempSDJKMapPack, lastTime).Forget();

            tempSDJKMapPack = MapManager.selectedMapPack;
            tempSDJKMap = MapManager.selectedMap;
            tempSongFile = MapManager.selectedMapInfo.songFile;
        }
    }
}
