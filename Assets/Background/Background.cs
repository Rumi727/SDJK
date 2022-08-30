using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Resource;
using SCKRM.Rhythm;
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

        public bool padeOut { get; set; } = false;

        CancellationTokenSource cancelSource = new CancellationTokenSource();
        Map.Map map;
        public override void OnCreate()
        {
            base.OnCreate();

            transform.SetSiblingIndex(0);
            map = MapManager.selectedMap;
        }

        string tempTexturePath = "";
        void Update()
        {
            if (!padeOut)
            {
                canvasGroup.alpha = canvasGroup.alpha.MoveTowards(1, 0.05f * Kernel.fpsUnscaledDeltaTime);

                {
                    string texturePath = map.globalEffect.background.GetValue(RhythmManager.currentBeatScreen).backgroundFile;
                    if (texturePath != tempTexturePath)
                    {
                        tempTexturePath = texturePath;

                        texturePath = PathTool.Combine(map.mapFilePathParent, texturePath);
                        image.sprite = ResourceManager.GetSprite(ResourceManager.GetTexture(texturePath, false, FilterMode.Bilinear, true, TextureMetaData.CompressionType.none));
                    }
                }

                if (image.sprite != null)
                    image.color = map.globalEffect.backgroundColor.GetValue(RhythmManager.currentBeatScreen);
                else
                    image.color = Color.black;
            }
            else
            {
                canvasGroup.alpha = canvasGroup.alpha.MoveTowards(0, 0.05f * Kernel.fpsUnscaledDeltaTime);

                if (canvasGroup.alpha <= 0)
                    Remove();
            }
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            tempTexturePath = "";
            padeOut = false;
            image.color = Color.black;
            canvasGroup.alpha = 0;
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
