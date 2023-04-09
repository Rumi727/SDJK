using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Resource;
using SCKRM.Rhythm;
using SCKRM.UI;
using SDJK.Map;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Effect
{
    [RequireComponent(typeof(Image)), RequireComponent(typeof(CanvasGroup))]
    public sealed class BackgroundEffectPrefab : UIObjectPoolingBase
    {
        public Image image => this.GetComponentFieldSave(_image); [SerializeField] Image _image;
        public CanvasGroup canvasGroup => this.GetComponentFieldSave(_canvasGroup); [SerializeField] CanvasGroup _canvasGroup;

        public EffectManager effectManager { get; private set; } = null;
        public MapFile map { get; private set; }

        public bool isRemoveQueue { get; set; } = false;

        bool refreshed = false;
        public void Refresh(EffectManager effectManager)
        {
            if (refreshed)
            {
                Remove();
                return;
            }

            map = effectManager.selectedMap;
            refreshed = true;

            this.effectManager = effectManager;
            TextureLoad().Forget();
        }

        float timeoutTimer = 0;
        string tempTexturePath = "";
        Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();
        void Update()
        {
            if (!isRemoveQueue && loadedSprites.Count > 0)
            {
                //1초동안 배경이 로딩되지 않았으면 알파값이 1이 될때까지 텍스쳐를 변경하지 않음
                if (timeoutTimer < 1 || (timeoutTimer >= 1 && canvasGroup.alpha >= 1))
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

                        textureChangeCancelSource.Cancel();
                        textureChangeCancelSource = new CancellationTokenSource();

                        TextureChange(texturePath).Forget();
                    }
                }
            }

            if (loadedSprites.Count > 0 && image.sprite != null)
            {
                canvasGroup.alpha = canvasGroup.alpha.MoveTowards(1, 0.05f * Kernel.fpsUnscaledSmoothDeltaTime);
                image.color = map.globalEffect.backgroundColor.GetValue(RhythmManager.currentBeatScreen);
            }
            else
            {
                //배경이 로딩된 후 1초가 지나게 되면 알파값을 강제로 변경
                if (timeoutTimer >= 1)
                {
                    canvasGroup.alpha = canvasGroup.alpha.MoveTowards(1, 0.05f * Kernel.fpsUnscaledSmoothDeltaTime);
                    image.color = Color.black;
                }
                else
                {
                    image.color = Color.clear;
                    timeoutTimer += Kernel.unscaledDeltaTime;
                }
            }
        }

        bool isTextureLoading = false;
        async UniTaskVoid TextureLoad()
        {
            isTextureLoading = true;

            for (int i = 0; i < map.globalEffect.background.Count; i++)
            {
                BackgroundEffectPair backgroundEffect = map.globalEffect.background[i].value;
                string background = backgroundEffect.backgroundFile;
                string backgroundNight = backgroundEffect.backgroundNightFile;

                string texturePath = PathUtility.Combine(map.mapFilePathParent, background);
                Texture2D texture = await ResourceManager.GetTextureAsync(texturePath, false, FilterMode.Bilinear, true, TextureMetaData.CompressionType.none);

                if (texture != null && !loadedSprites.ContainsKey(background))
                    loadedSprites.Add(background, ResourceManager.GetSprite(texture));

                if (!Kernel.isPlaying || isRemoved || IsDestroyed())
                {
                    isTextureLoading = false;
                    TextureDestroy().Forget();

                    return;
                }

                string nightTexturePath = PathUtility.Combine(map.mapFilePathParent, backgroundNight);
                Texture2D nightTexture = await ResourceManager.GetTextureAsync(nightTexturePath, false, FilterMode.Bilinear, true, TextureMetaData.CompressionType.none);

                if (nightTexture != null && !loadedSprites.ContainsKey(backgroundNight))
                    loadedSprites.Add(backgroundNight, ResourceManager.GetSprite(nightTexture));

                if (!Kernel.isPlaying || isRemoved || IsDestroyed())
                {
                    isTextureLoading = false;
                    TextureDestroy().Forget();

                    return;
                }
            }

            isTextureLoading = false;
        }

        CancellationTokenSource textureChangeCancelSource = new CancellationTokenSource();
        async UniTaskVoid TextureChange(string texturePath)
        {
            await UniTask.WaitUntil(() => loadedSprites.ContainsKey(texturePath), PlayerLoopTiming.Update, textureChangeCancelSource.Token);
            image.sprite = loadedSprites[texturePath];
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            tempTexturePath = "";
            image.color = Color.black;
            canvasGroup.alpha = 0;
            refreshed = false;
            effectManager = null;
            isTextureLoading = false;
            isRemoveQueue = false;
            timeoutTimer = 0;

            TextureDestroy().Forget();

            return true;
        }

        protected override void OnDestroy() => TextureDestroy().Forget();

        async UniTaskVoid TextureDestroy()
        {
            await UniTask.WaitUntil(() => !isTextureLoading);

            foreach (var item in loadedSprites)
            {
                if (item.Value != null)
                {
                    if (Kernel.isPlaying)
                    {
                        Destroy(item.Value.texture);
                        Destroy(item.Value);
                    }
                    else
                    {
                        DestroyImmediate(item.Value.texture);
                        DestroyImmediate(item.Value);
                    }
                }
            }

            loadedSprites.Clear();
        }
    }
}
