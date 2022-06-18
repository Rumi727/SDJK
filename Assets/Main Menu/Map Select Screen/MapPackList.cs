using SCKRM.Object;
using SCKRM.UI;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK
{
    public class MapPackList : UI
    {
        [SerializeField] RectTransform content;



        protected override void Awake() => MapManager.mapLoadingEnd += ReloadData;
        protected override void OnDestroy() => MapManager.mapLoadingEnd -= ReloadData;



        List<MapPackListMapPack> mapSelectScreenMapPacks = new List<MapPackListMapPack>();
        void ReloadData()
        {
            for (int i = 0; i < mapSelectScreenMapPacks.Count; i++)
                mapSelectScreenMapPacks[i].Remove();

            mapSelectScreenMapPacks.Clear();

            for (int i = 0; i < MapManager.currentMapPacks.Count; i++)
            {
                MapPackListMapPack mapPackListMapPack = (MapPackListMapPack)ObjectPoolingSystem.ObjectCreate("map_select_screen.map_pack", content).monoBehaviour;
                mapPackListMapPack.ConfigureCell(MapManager.currentMapPacks[i]).Forget();

                mapSelectScreenMapPacks.Add(mapPackListMapPack);
            }
        }
    }
}
