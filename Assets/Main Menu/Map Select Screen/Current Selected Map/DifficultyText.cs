using SCKRM;
using SDJK.Map;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.MainMenu.MapSelectScreen
{
    public sealed class DifficultyText : SCKRM.UI.UIBase
    {
        [SerializeField] ColorBand gradient;

        [SerializeField] Image background;
        [SerializeField] TMP_Text text;

        MapFile lastMap;
        void Update()
        {
            MapFile map = MapManager.selectedMap;
            if (lastMap != map)
            {
                background.color = gradient.Evaluate((float)(map.difficulty / 10d));
                text.text = map.difficulty.Round(2).ToString();

                lastMap = map;
            }
        }
    }
}
