using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Resource;
using SCKRM.UI;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SDJK.MapSelectScreen
{
    public class MapPackListMapPack : UIObjectPooling, IPointerClickHandler
    {
        public static bool isTextureLoading { get; private set; } = false;

        [SerializeField, NotNull] Image outline;
        [SerializeField, NotNull] Image background;
        [SerializeField, NotNull] TMP_Text songName;
        [SerializeField, NotNull] TMP_Text artist;

        public override void OnCreate()
        {
            base.OnCreate();

            rectTransform.offsetMin = new Vector2(100, rectTransform.offsetMin.y);
            rectTransform.offsetMax = new Vector2(40, rectTransform.offsetMax.y);
        }

        MapPack mapPack;
        int mapPackIndex;
        void Update()
        {
            if (mapPack == MapManager.selectedMapPack)
            {
                outline.color = outline.color.MoveTowards(Color.white, 0.1f * Kernel.fpsUnscaledDeltaTime);
                rectTransform.offsetMin = rectTransform.offsetMin.Lerp(new Vector2(0, rectTransform.offsetMin.y), 0.2f * Kernel.fpsUnscaledDeltaTime);

                float viewportHeight = MapPackList.instance.viewport.rect.height;
                float contentHeight = MapPackList.instance.content.rect.height;

                float thisPos = -rectTransform.anchoredPosition.y + (rectTransform.rect.height * 0.5f);
                MapPackList.contentPosY = (thisPos - (viewportHeight * 0.5f)).Clamp(0, contentHeight - viewportHeight);
            }
            else
            {
                outline.color = outline.color.MoveTowards(new Color(1, 1, 1, 0), 0.1f * Kernel.fpsUnscaledDeltaTime);
                rectTransform.offsetMin = rectTransform.offsetMin.Lerp(new Vector2(100, rectTransform.offsetMin.y), 0.2f * Kernel.fpsUnscaledDeltaTime);
            }
        }

        CancellationTokenSource cancelSource = new CancellationTokenSource();
        public async UniTaskVoid ConfigureCell(MapPackList mapPackList, MapPack mapPack, int mapPackIndex)
        {
            this.mapPack = mapPack;
            this.mapPackIndex = mapPackIndex;

            Map.Map firstMap = mapPack.maps[0];

            background.sprite = null;
            songName.text = firstMap.info.songName;
            artist.text = firstMap.info.artist;

            if (await UniTask.WaitUntil(() => !isTextureLoading, PlayerLoopTiming.Update, cancelSource.Token).SuppressCancellationThrow())
                return;

            isTextureLoading = true;

            if (background.sprite != null)
                Destroy(background.sprite);

            string texturePath = PathTool.Combine(firstMap.mapFilePathParent, firstMap.info.backgroundFile);
            background.sprite = ResourceManager.GetSprite(await ResourceManager.GetTextureAsync(texturePath, false, FilterMode.Bilinear, true, TextureMetaData.CompressionType.none));

            isTextureLoading = false;
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            cancelSource.Cancel();
            cancelSource.Dispose();
            cancelSource = new CancellationTokenSource();

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

        public void OnPointerClick(PointerEventData eventData) => MapManager.selectedMapPackIndex = mapPackIndex;
    }
}
