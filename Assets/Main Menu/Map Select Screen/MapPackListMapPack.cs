using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Object;
using SCKRM.Renderer;
using SCKRM.Resource;
using SCKRM.UI;
using SCKRM.UI.Layout;
using SDJK.Map;
using SDJK.Ruleset;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SDJK.MainMenu.MapSelectScreen
{
    public class MapPackListMapPack : UIObjectPoolingBase, IPointerClickHandler
    {
        public static bool isTextureLoading { get; private set; } = false;

        [SerializeField, NotNull] VerticalLayout verticalLayout;
        [SerializeField, NotNull] Image outline;
        [SerializeField, NotNull] Image background;
        [SerializeField, NotNull] TMP_Text songName;
        [SerializeField, NotNull] TMP_Text artist;
        [SerializeField] CustomSpriteRendererBase rulesetIcon;
        [SerializeField] Image rulesetIconBackground;
        [SerializeField] RectTransform rulesetIconRectTransform;
        [SerializeField] bool isMap = false;
        [SerializeField] Transform rulesetList;

        [SerializeField, NotNull] ColorBand difficultyGradient;
        [SerializeField] Image difficultyBackground;
        [SerializeField] TMP_Text difficultyText;

        public GameObject viewport;

        public override void OnCreate()
        {
            base.OnCreate();

            rectTransform.offsetMin = new Vector2(100, rectTransform.offsetMin.y);
            rectTransform.offsetMax = new Vector2(40, rectTransform.offsetMax.y);

            background.color = Color.gray;
        }

        MapPackList mapPackList;
        MapPack mapPack;
        int mapPackIndex;
        MapFile map;
        int mapIndex;
        public void Update()
        {
            if (isMap)
                verticalLayout.padding.left = (int)(rulesetIconRectTransform.anchoredPosition.x * 2 + rulesetIconRectTransform.rect.width);

            if ((mapPack == MapManager.selectedMapPack && MainMenu.currentScreenMode == ScreenMode.mapPackSelect && !isMap) || (map == MapManager.selectedMap && MainMenu.currentScreenMode == ScreenMode.mapSelect && isMap))
            {
                outline.color = outline.color.MoveTowards(Color.white, 0.1f * Kernel.fpsUnscaledSmoothDeltaTime);
                rectTransform.offsetMin = rectTransform.offsetMin.Lerp(new Vector2(0, rectTransform.offsetMin.y), 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);

                float viewportHeight = mapPackList.viewport.rect.height;
                float contentHeight = mapPackList.content.rect.height;

                float thisPos = -rectTransform.anchoredPosition.y + (rectTransform.rect.height * 0.5f);
                mapPackList.contentPosY = (thisPos - (viewportHeight * 0.5f)).Clamp(0, contentHeight - viewportHeight);
            }
            else
            {
                outline.color = outline.color.MoveTowards(new Color(1, 1, 1, 0), 0.1f * Kernel.fpsUnscaledSmoothDeltaTime);
                rectTransform.offsetMin = rectTransform.offsetMin.Lerp(new Vector2(100, rectTransform.offsetMin.y), 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
            }
        }

        List<MapPackListRulesetIcon> mapPackListRulesetIcons = new List<MapPackListRulesetIcon>();
        CancellationTokenSource cancelSource = new CancellationTokenSource();
        public async UniTaskVoid ConfigureCell(MapPackList mapPackList, MapPack mapPack, int mapPackIndex, MapFile map, int mapIndex)
        {
            this.mapPackList = mapPackList;
            this.mapPack = mapPack;
            this.mapPackIndex = mapPackIndex;
            this.map = map;
            this.mapIndex = mapIndex;

            MapFile selectedMap = null;
            if (!isMap)
            {
                for (int i = 0; i < mapPack.maps.Count; i++)
                {
                    MapFile loopMap = mapPack.maps[i];
                    if (RulesetManager.selectedRuleset.IsCompatibleRuleset(loopMap.info.ruleset))
                    {
                        selectedMap = loopMap;
                        break;
                    }
                }
            }
            else
                selectedMap = map;

            if (!isMap)
            {
                songName.text = selectedMap.info.songName;
                artist.text = selectedMap.info.artist;

                //Ruleset 아이콘
                for (int i = 0; i < mapPack.maps.Count; i++)
                {
                    MapFile map2 = mapPack.maps[i];
                    string ruleset = map2.info.ruleset;
                    MapPackListRulesetIcon icon = (MapPackListRulesetIcon)ObjectPoolingSystem.ObjectCreate("map_select_screen.map_pack_ruleset_icon", rulesetList).monoBehaviour;

                    if (!RulesetManager.selectedRuleset.IsCompatibleRuleset(ruleset))
                        icon.canvasGroup.alpha = 0.4f;

                    icon.icon.nameSpaceIndexTypePathPair = RulesetManager.FindRuleset(ruleset)?.icon ?? "";
                    icon.icon.Refresh();

                    icon.iconBackground.color = difficultyGradient.Evaluate((float)(map2.difficulty.Average() / 10d));
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

                double difficulty = map.difficulty.Average();

                Color color = difficultyGradient.Evaluate((float)(difficulty / 10d));
                difficultyBackground.color = color;
                rulesetIconBackground.color = color;

                difficultyText.text = difficulty.ToString("0.00");
            }

            if (await UniTask.WaitUntil(() => !Kernel.isPlaying || isRemoved || IsDestroyed() || (!isTextureLoading && !IsOccluded()), PlayerLoopTiming.Update, cancelSource.Token).SuppressCancellationThrow())
            {
                TextureDestroy();
                return;
            }

            if (!Kernel.isPlaying || isRemoved || IsDestroyed())
            {
                TextureDestroy();
                return;
            }

            isTextureLoading = true;

            try
            {
                TextureDestroy();

                if (selectedMap.globalEffect.background.Count > 0)
                {
                    string texturePath = selectedMap.globalEffect.background[0].value.backgroundFile;
                    texturePath = PathUtility.Combine(selectedMap.mapFilePathParent, texturePath);

                    Texture2D texture = await ResourceManager.GetTextureAsync(texturePath, false, FilterMode.Bilinear, true, TextureMetaData.CompressionType.none);
                    if (!Kernel.isPlaying || isRemoved || IsDestroyed())
                    {
                        TextureDestroy();
                        return;
                    }

                    if (texture != null)
                    {
                        background.sprite = ResourceManager.GetSprite(texture);
                        background.color = Color.white;
                    }
                    else
                    {
                        background.sprite = null;
                        background.color = Color.gray;
                    }
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
        }

        protected override void OnDestroy() => TextureDestroy();

        public bool IsOccluded()
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

            TextureDestroy();

            background.sprite = null;
            background.color = Color.gray;

            songName.text = "";
            artist.text = "";

            return true;
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

        void TextureDestroy()
        {
            if (background.sprite != null)
            {
                DestroyImmediate(background.sprite.texture);
                DestroyImmediate(background.sprite);
            }
        }
    }
}
