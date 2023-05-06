using SCKRM;
using SDJK.Map;
using System.Linq;
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
                double difficulty = map.difficulty.Average();

                background.color = gradient.Evaluate((float)(difficulty / 10d));
                text.text = difficulty.ToString("0.00");

                lastMap = map;
            }
        }
    }
}
