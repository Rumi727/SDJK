using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Resource;
using SCKRM.UI;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SDJK
{
    [RequireComponent(typeof(Image)), RequireComponent(typeof(CanvasGroup))]
    public sealed class Background : UIObjectPooling
    {
        public Image image => this.GetComponentFieldSave(_image); [SerializeField] Image _image;
        public CanvasGroup canvasGroup => this.GetComponentFieldSave(_canvasGroup); [SerializeField] CanvasGroup _canvasGroup;

        CancellationTokenSource cancelSource = new CancellationTokenSource();
        public override async void OnCreate()
        {
            base.OnCreate();

            transform.SetSiblingIndex(0);

            SDJKMap map = MapManager.selectedMap;
            string texturePath = PathTool.Combine(map.mapFilePathParent, map.info.backgroundFile);
            if (ResourceManager.FileExtensionExists(texturePath, out texturePath, ResourceManager.textureExtension))
            {
                using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(texturePath))
                {
                    if ((await www.SendWebRequest().ToUniTask(null, PlayerLoopTiming.Update, cancelSource.Token).SuppressCancellationThrow()).IsCanceled)
                        return;

                    image.color = Color.white;
                    image.sprite = ResourceManager.GetSprite(DownloadHandlerTexture.GetContent(www));
                }
            }
            else
                Remove();
        }

        public async UniTaskVoid PadeOut()
        {
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha = canvasGroup.alpha.MoveTowards(0, 0.05f * Kernel.fpsUnscaledDeltaTime);
                await UniTask.NextFrame();

                if (this == null)
                {
                    TextureDestroy();
                    return;
                }
            }

            Remove();
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            image.color = Color.black;
            canvasGroup.alpha = 1;
            TextureDestroy();

            return true;
        }

        void TextureDestroy()
        {
            cancelSource.Cancel();
            cancelSource.Dispose();
            cancelSource = new CancellationTokenSource();

            if (image.sprite != null)
            {
                Destroy(image.sprite.texture);
                Destroy(image.sprite);
            }
        }
    }
}
