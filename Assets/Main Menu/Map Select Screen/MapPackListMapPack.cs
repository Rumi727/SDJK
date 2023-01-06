using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Object;
using SCKRM.Renderer;
using SCKRM.Resource;
using SCKRM.UI;
using SCKRM.UI.Layout;
using SDJK.Map;
using SDJK.Ruleset;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SDJK.MainMenu.MapSelectScreen
{
    public class MapPackListMapPack : UIObjectPooling, IPointerClickHandler
    {
        public static bool isTextureLoading { get; private set; } = false;

        [SerializeField, NotNull] VerticalLayout verticalLayout;
        [SerializeField, NotNull] Image outline;
        [SerializeField, NotNull] Image background;
        [SerializeField, NotNull] TMP_Text songName;
        [SerializeField, NotNull] TMP_Text artist;
        [SerializeField] CustomAllSpriteRenderer rulesetIcon;
        [SerializeField] RectTransform rulesetIconRectTransform;
        [SerializeField] bool isMap = false;
        [SerializeField] Transform rulesetList;

        public override void OnCreate()
        {
            base.OnCreate();

            rectTransform.offsetMin = new Vector2(100, rectTransform.offsetMin.y);
            rectTransform.offsetMax = new Vector2(40, rectTransform.offsetMax.y);
        }

        MapPackList mapPackList;
        MapPack mapPack;
        int mapPackIndex;
        Map.MapFile map;
        int mapIndex;
        void Update()
        {
            if (isMap)
                verticalLayout.padding.left = (int)(rulesetIconRectTransform.anchoredPosition.x * 2 + rulesetIconRectTransform.rect.width);

            if ((mapPack == MapManager.selectedMapPack && MainMenu.currentScreenMode == ScreenMode.mapPackSelect && !isMap) || (map == MapManager.selectedMap && MainMenu.currentScreenMode == ScreenMode.mapSelect && isMap))
            {
                outline.color = outline.color.MoveTowards(Color.white, 0.1f * Kernel.fpsUnscaledDeltaTime);
                rectTransform.offsetMin = rectTransform.offsetMin.Lerp(new Vector2(0, rectTransform.offsetMin.y), 0.2f * Kernel.fpsUnscaledDeltaTime);

                float viewportHeight = mapPackList.viewport.rect.height;
                float contentHeight = mapPackList.content.rect.height;

                float thisPos = -rectTransform.anchoredPosition.y + (rectTransform.rect.height * 0.5f);
                mapPackList.contentPosY = (thisPos - (viewportHeight * 0.5f)).Clamp(0, contentHeight - viewportHeight);
            }
            else
            {
                outline.color = outline.color.MoveTowards(new Color(1, 1, 1, 0), 0.1f * Kernel.fpsUnscaledDeltaTime);
                rectTransform.offsetMin = rectTransform.offsetMin.Lerp(new Vector2(100, rectTransform.offsetMin.y), 0.2f * Kernel.fpsUnscaledDeltaTime);
            }
        }

        List<MapPackListRulesetIcon> mapPackListRulesetIcons = new List<MapPackListRulesetIcon>();
        CancellationTokenSource cancelSource = new CancellationTokenSource();
        public async UniTaskVoid ConfigureCell(MapPackList mapPackList, MapPack mapPack, int mapPackIndex, Map.MapFile map, int mapIndex)
        {
            this.mapPackList = mapPackList;
            this.mapPack = mapPack;
            this.mapPackIndex = mapPackIndex;
            this.map = map;
            this.mapIndex = mapIndex;

            Map.MapFile selectedMap;
            if (!isMap)
                selectedMap = mapPack.maps[0];
            else
                selectedMap = map;

            background.sprite = null;

            if (!isMap)
            {
                songName.text = selectedMap.info.songName;
                artist.text = selectedMap.info.artist;

                //Ruleset 아이콘
                for (int i = 0; i < mapPack.maps.Count; i++)
                {
                    MapPackListRulesetIcon icon = (MapPackListRulesetIcon)ObjectPoolingSystem.ObjectCreate("map_select_screen.map_pack_ruleset_icon", rulesetList).monoBehaviour;

                    icon.icon.nameSpaceIndexTypePathPair = RulesetManager.FindRuleset(mapPack.maps[i].info.ruleset)?.icon ?? "";
                    icon.icon.Refresh();

                    mapPackListRulesetIcons.Add(icon);
                }
            }
            else
            {
                songName.text = selectedMap.info.difficultyLabel;
                artist.text = selectedMap.info.author;

                //Ruleset 아이콘
                rulesetIcon.nameSpaceIndexTypePathPair = RulesetManager.FindRuleset(selectedMap.info.ruleset)?.icon ?? "";
                rulesetIcon.Refresh();
            }

            if (await UniTask.WaitUntil(() => !Kernel.isPlaying || isRemoved || IsDestroyed() || (!isTextureLoading && !IsOccluded()), PlayerLoopTiming.Update, cancelSource.Token).SuppressCancellationThrow())
                return;

            if (!Kernel.isPlaying || isRemoved || IsDestroyed())
                return;

            isTextureLoading = true;

            try
            {
                if (background.sprite != null)
                {
                    Destroy(background.sprite.texture);
                    Destroy(background.sprite);
                }

                if (selectedMap.globalEffect.background.Count > 0)
                {
                    string texturePath = selectedMap.globalEffect.background[0].value.backgroundFile;
                    texturePath = PathTool.Combine(selectedMap.mapFilePathParent, texturePath);
                    background.sprite = ResourceManager.GetSprite(await ResourceManager.GetTextureAsync(texturePath, false, FilterMode.Bilinear, true, TextureMetaData.CompressionType.none));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                isTextureLoading = false;
            }

            while (true)
            {
                if (!Kernel.isPlaying || isRemoved || IsDestroyed())
                    return;

                bool active = !IsOccluded();
                if (active != gameObject.activeSelf)
                    gameObject.SetActive(active);

                await UniTask.NextFrame();
            }
        }

        private bool IsOccluded()
        {
            bool top = mapPackList.rectTransformTool.worldCorners.topLeft.y < rectTransformTool.worldCorners.bottomLeft.y - 5;
            bool bottom = mapPackList.rectTransformTool.worldCorners.bottomLeft.y > rectTransformTool.worldCorners.topLeft.y + 5;

            return top || bottom;
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            cancelSource.Cancel();
            cancelSource.Dispose();
            cancelSource = new CancellationTokenSource();

            for (int i = 0; i < mapPackListRulesetIcons.Count; i++)
                mapPackListRulesetIcons[i].Remove();

            if (background.sprite != null)
            {
                Destroy(background.sprite.texture);
                Destroy(background.sprite);
            }

            background.sprite = null;
            songName.text = "";
            artist.text = "";

            return true;
        }

        protected override void OnDestroy()
        {
            if (background.sprite != null)
            {
                Destroy(background.sprite.texture);
                Destroy(background.sprite);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isMap)
            {
                if (MapManager.selectedMapPackIndex != mapPackIndex)
                    MapManager.selectedMapPackIndex = mapPackIndex;
                else
                    MainMenu.NextScreen();
            }
            else
            {
                if (MapManager.selectedMapIndex != mapIndex)
                    MapManager.selectedMapIndex = mapIndex;
                else
                    MainMenu.NextScreen();
            }
        }
    }
}
