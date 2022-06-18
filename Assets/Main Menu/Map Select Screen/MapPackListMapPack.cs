using Cysharp.Threading.Tasks;
using PolyAndCode.UI;
using SCKRM;
using SCKRM.Renderer;
using SCKRM.Resource;
using SCKRM.UI;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SDJK
{
    public class MapPackListMapPack : UIObjectPooling
    {
        public static bool isTextureLoading { get; private set; } = false;
        [SerializeField] Image background;
        [SerializeField] TMP_Text songName;
        [SerializeField] TMP_Text artist;

        public async UniTaskVoid ConfigureCell(SDJKMapPack mapPack)
        {
            SDJKMap firstMap = mapPack.maps[0];

            background.sprite = null;
            songName.text = firstMap.info.songName;
            artist.text = firstMap.info.artist;

            CancellationToken cancelToken = this.GetCancellationTokenOnDestroy();
            if (await UniTask.WaitUntil(() => !isTextureLoading, PlayerLoopTiming.Update, cancelToken).SuppressCancellationThrow())
                return;

            isTextureLoading = true;

            if (background.sprite != null)
                Destroy(background.sprite);

            string texturePath = PathTool.Combine(firstMap.mapFilePathParent, firstMap.info.backgroundFile);
            if (ResourceManager.FileExtensionExists(texturePath, out texturePath, ResourceManager.textureExtension))
            {
                using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(texturePath))
                {
                    if ((await www.SendWebRequest().ToUniTask(null, PlayerLoopTiming.Update, cancelToken).SuppressCancellationThrow()).IsCanceled)
                    {
                        isTextureLoading = false;
                        return;
                    }

                    background.sprite = ResourceManager.GetSprite(DownloadHandlerTexture.GetContent(www));
                }
            }

            isTextureLoading = false;
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

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
    }
}
