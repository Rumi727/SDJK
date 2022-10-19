using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Object;
using SCKRM.UI;
using SCKRM.UI.Layout;
using SDJK.Map;
using SDJK.Ruleset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SDJK.MapSelectScreen
{
    public class MapPackList : UI, IBeginDragHandler, IScrollHandler
    {
        public bool contentPosLock { get; private set; }
        public float contentPosY { get; set; }



        [SerializeField, NotNull] ScrollRect scrollRect;

        [SerializeField, NotNull] VerticalLayout _verticalLayout; public VerticalLayout verticalLayout => _verticalLayout;
        [SerializeField, NotNull] RectTransform _viewport; public RectTransform viewport => _viewport;
        [SerializeField, NotNull] RectTransform _content; public RectTransform content => _content;

        [SerializeField] bool isMapList = false;



        protected override void Awake()
        {
            MapManager.mapLoadingEnd += ReloadData;
            ReloadData();
        }

        protected override void OnEnable() => rectTransform.anchoredPosition = new Vector2(rectTransform.rect.width, 0);

        Map.Map lastMap;
        MapPack lastMapPack;
        void Update()
        {
            if ((MainMenu.currentScreenMode == ScreenMode.mapPackSelect && !isMapList) || (MainMenu.currentScreenMode == ScreenMode.mapSelect && isMapList))
            {
                rectTransform.anchoredPosition = rectTransform.anchoredPosition.Lerp(new Vector2(0, 0), 0.2f * Kernel.fpsUnscaledDeltaTime);

                if (!content.gameObject.activeSelf)
                    content.gameObject.SetActive(true);
            }
            else
            {
                Rect rect = rectTransform.rect;
                rectTransform.anchoredPosition = rectTransform.anchoredPosition.MoveTowards(new Vector2(rect.width, 0), 100 * Kernel.fpsUnscaledDeltaTime);

                if (content.gameObject.activeSelf && rectTransform.anchoredPosition.x >= rect.width)
                    content.gameObject.SetActive(false);
            }

            if (lastMap != MapManager.selectedMap)
            {
                contentPosLock = false;
                lastMap = MapManager.selectedMap;
            }

            if (isMapList && lastMapPack != MapManager.selectedMapPack)
            {
                ReloadData();
                lastMapPack = MapManager.selectedMapPack;
            }

            if (content.rect.height > viewport.rect.height && !contentPosLock)
                content.anchoredPosition = content.anchoredPosition.Lerp(new Vector2(0, contentPosY), 0.2f * Kernel.fpsUnscaledDeltaTime);
        }

        protected override void OnDestroy() => MapManager.mapLoadingEnd -= ReloadData;



        List<MapPackListMapPack> mapSelectScreenMapPacks = new List<MapPackListMapPack>();
        async void ReloadData()
        {
            for (int i = 0; i < mapSelectScreenMapPacks.Count; i++)
                mapSelectScreenMapPacks[i].Remove();

            mapSelectScreenMapPacks.Clear();

            if (!isMapList)
            {
                for (int i = 0; i < MapManager.currentMapPacks.Count; i++)
                {
                    MapPack mapPack = MapManager.currentMapPacks[i];
                    for (int j = 0; j < mapPack.maps.Count; j++)
                    {
                        Map.Map map = mapPack.maps[j];
                        if (RulesetManager.selectedRuleset.IsCompatibleRuleset(map.info.mode))
                        {
                            MapPackListMapPack mapPackListMapPack = (MapPackListMapPack)ObjectPoolingSystem.ObjectCreate("map_select_screen.map_pack", _content).monoBehaviour;
                            mapPackListMapPack.ConfigureCell(this, mapPack, i, null, 0).Forget();

                            mapSelectScreenMapPacks.Add(mapPackListMapPack);

                            if (await UniTask.NextFrame(AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                                return;

                            break;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < MapManager.selectedMapPack.maps.Count; i++)
                {
                    Map.Map map = MapManager.selectedMapPack.maps[i];
                    if (!RulesetManager.selectedRuleset.IsCompatibleRuleset(map.info.mode))
                        continue;

                    MapPackListMapPack mapPackListMapPack = (MapPackListMapPack)ObjectPoolingSystem.ObjectCreate("map_select_screen.map", _content).monoBehaviour;
                    mapPackListMapPack.ConfigureCell(this, null, 0, map, i).Forget();

                    mapSelectScreenMapPacks.Add(mapPackListMapPack);

                    if (await UniTask.NextFrame(AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                        return;
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData) => contentPosLock = true;
        public void OnScroll(PointerEventData eventData) => contentPosLock = true;
    }
}
