using SCKRM;
using SCKRM.Object;
using SCKRM.UI;
using SCKRM.UI.Layout;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SDJK.MapSelectScreen
{
    public class MapPackList : UIManager<MapPackList>, IBeginDragHandler, IScrollHandler
    {
        public static bool contentPosLock { get; private set; }
        public static float contentPosY { get; set; }



        [SerializeField, NotNull] ScrollRect scrollRect;

        [SerializeField, NotNull] VerticalLayout _verticalLayout; public VerticalLayout verticalLayout => _verticalLayout;
        [SerializeField, NotNull] RectTransform _viewport; public RectTransform viewport => _viewport;
        [SerializeField, NotNull] RectTransform _content; public RectTransform content => _content;



        protected override void Awake()
        {
            if (SingletonCheck(this))
            {
                MapManager.mapLoadingEnd += ReloadData;
                ReloadData();
            }
        }

        Map.Map lastMap;
        void Update()
        {
            if (lastMap != MapManager.selectedMap)
            {
                contentPosLock = false;
                lastMap = MapManager.selectedMap;
            }

            if (content.rect.height > viewport.rect.height && !contentPosLock)
                content.anchoredPosition = content.anchoredPosition.Lerp(new Vector2(0, contentPosY), 0.2f * Kernel.fpsUnscaledDeltaTime);
        }

        protected override void OnDestroy() => MapManager.mapLoadingEnd -= ReloadData;



        List<MapPackListMapPack> mapSelectScreenMapPacks = new List<MapPackListMapPack>();
        void ReloadData()
        {
            for (int i = 0; i < mapSelectScreenMapPacks.Count; i++)
                mapSelectScreenMapPacks[i].Remove();

            mapSelectScreenMapPacks.Clear();

            for (int i = 0; i < MapManager.currentMapPacks.Count; i++)
            {
                MapPackListMapPack mapPackListMapPack = (MapPackListMapPack)ObjectPoolingSystem.ObjectCreate("map_select_screen.map_pack", _content).monoBehaviour;
                mapPackListMapPack.ConfigureCell(this, MapManager.currentMapPacks[i], i).Forget();

                mapSelectScreenMapPacks.Add(mapPackListMapPack);
            }
        }

        public void OnBeginDrag(PointerEventData eventData) => contentPosLock = true;
        public void OnScroll(PointerEventData eventData) => contentPosLock = true;
    }
}
