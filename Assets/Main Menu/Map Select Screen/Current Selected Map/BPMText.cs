using SCKRM;
using SDJK.Map;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SDJK.MainMenu.MapSelectScreen
{
    public sealed class BPMText : SCKRM.UI.UIBase
    {
        [SerializeField] TMP_Text text;

        MapFile lastMap;
        void Update()
        {
            MapFile map = MapManager.selectedMap;
            if (lastMap != map)
            {
                double min = map.globalEffect.bpm.Min(x => x.value).Round(3);
                double max = map.globalEffect.bpm.Max(x => x.value).Round(3);

                if (min != max)
                    text.text = min + " - " + max;
                else
                    text.text = min.ToString();

                lastMap = map;
            }
        }
    }
}
