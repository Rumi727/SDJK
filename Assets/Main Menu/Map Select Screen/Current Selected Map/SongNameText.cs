using SDJK.Map;
using TMPro;
using UnityEngine;

namespace SDJK.MainMenu.MapSelectScreen
{
    public sealed class SongNameText : SCKRM.UI.UIBase
    {
        [SerializeField] TMP_Text text;

        MapFile lastMap;
        void Update()
        {
            MapFile map = MapManager.selectedMap;
            if (lastMap != map)
            {
                text.text = map.info.songName;
                lastMap = map;
            }
        }
    }
}
