using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Resource;
using SCKRM.Rhythm;
using SCKRM.UI;
using SDJK.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
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

            TextureLoad().Forget();
        }

        string tempTexturePath = "";
        Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();
        void Update()
        {
            if (!padeOut)
            {
                if (loadedSprites.Count > 0)
                {
                    canvasGroup.alpha = canvasGroup.alpha.MoveTowards(1, 0.05f * Kernel.fpsUnscaledDeltaTime);

                    {
                        DateTime now = DateTime.Now;
                        string texturePath;
                        if (now.Hour >= 0 && now.Hour < 4)
                        {
                            texturePath = map.globalEffect.background.GetValue(RhythmManager.currentBeatScreen).backgroundNightFile;

                            if (string.IsNullOrEmpty(texturePath))
                                texturePath = map.globalEffect.background.GetValue(RhythmManager.currentBeatScreen).backgroundFile;
                        }
                        else
                            texturePath = map.globalEffect.background.GetValue(RhythmManager.currentBeatScreen).backgroundFile;

                        if (texturePath != tempTexturePath)
                        {
                            tempTexturePath = texturePath;
                            TextureChange(texturePath).Forget();
                        }
                    }

                    if (image.sprite != null)
                        image.color = map.globalEffect.backgroundColor.GetValue(RhythmManager.currentBeatScreen);
                    else
                        image.color = Color.black;
                }
            }
            else
            {
                canvasGroup.alpha = canvasGroup.alpha.MoveTowards(0, 0.05f * Kernel.fpsUnscaledDeltaTime);

                if (canvasGroup.alpha <= 0)
                    Remove();
            }
        }


        async UniTaskVoid TextureLoad()
        {
            //이거 안하면 씬 이동할때 원인 모를 예외 남;
            await UniTask.NextFrame();

            for (int i = 0; i < map.globalEffect.background.Count; i++)
            {
                BackgroundEffect backgroundEffect = map.globalEffect.background[i].value;
                string background = backgroundEffect.backgroundFile;
                string backgroundNight = backgroundEffect.backgroundNightFile;

                string texturePath = PathTool.Combine(map.mapFilePathParent, background);
                Texture2D texture = await ResourceManager.GetTextureAsync(texturePath, false, FilterMode.Bilinear, true, TextureMetaData.CompressionType.none);

                string nightTexturePath = PathTool.Combine(map.mapFilePathParent, backgroundNight);
                Texture2D nightTexture = await ResourceManager.GetTextureAsync(nightTexturePath, false, FilterMode.Bilinear, true, TextureMetaData.CompressionType.none);

                if (texture != null && !loadedSprites.ContainsKey(background))
                    loadedSprites.Add(background, ResourceManager.GetSprite(texture));
                if (nightTexture != null && !loadedSprites.ContainsKey(backgroundNight))
                    loadedSprites.Add(backgroundNight, ResourceManager.GetSprite(nightTexture));
            }
        }

        async UniTaskVoid TextureChange(string texturePath)
        {
            await UniTask.WaitUntil(() => loadedSprites.ContainsKey(texturePath));
            image.sprite = loadedSprites[texturePath];
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

        protected override void OnDestroy() => TextureDestroy();

        void TextureDestroy()
        {
            cancelSource.Cancel();
            cancelSource.Dispose();
            cancelSource = new CancellationTokenSource();

            foreach (var item in loadedSprites)
            {
                Destroy(item.Value.texture);
                Destroy(item.Value);
            }

            loadedSprites.Clear();
        }
    }
}
