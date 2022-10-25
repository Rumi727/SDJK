using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Resource;
using SCKRM.Rhythm;
using SCKRM.UI;
using SDJK.Map;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK
{
    [RequireComponent(typeof(Image)), RequireComponent(typeof(CanvasGroup))]
    public sealed class BackgroundEffectPrefab : UIObjectPooling
    {
        public Image image => this.GetComponentFieldSave(_image); [SerializeField] Image _image;
        public CanvasGroup canvasGroup => this.GetComponentFieldSave(_canvasGroup); [SerializeField] CanvasGroup _canvasGroup;

        public bool padeOut { get; set; } = false;

        public override void OnCreate()
        {
            base.OnCreate();
            transform.SetSiblingIndex(0);
        }

        bool refreshed = false;
        public void Refresh(Map.Map map)
        {
            if (refreshed)
            {
                Remove();
                return;
            }

            refreshed = true;

            this.map = map;
            TextureLoad().Forget();
        }

        Map.Map map;
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


        bool isTextureLoading = false;
        async UniTaskVoid TextureLoad()
        {
            isTextureLoading = true;

            for (int i = 0; i < map.globalEffect.background.Count; i++)
            {
                BackgroundEffectPair backgroundEffect = map.globalEffect.background[i].value;
                string background = backgroundEffect.backgroundFile;
                string backgroundNight = backgroundEffect.backgroundNightFile;

                string texturePath = PathTool.Combine(map.mapFilePathParent, background);
                Texture2D texture = await ResourceManager.GetTextureAsync(texturePath, false, FilterMode.Bilinear, true, TextureMetaData.CompressionType.none);

                if (isRemoved)
                    return;

                string nightTexturePath = PathTool.Combine(map.mapFilePathParent, backgroundNight);
                Texture2D nightTexture = await ResourceManager.GetTextureAsync(nightTexturePath, false, FilterMode.Bilinear, true, TextureMetaData.CompressionType.none);

                if (isRemoved)
                    return;

                if (texture != null && !loadedSprites.ContainsKey(background))
                    loadedSprites.Add(background, ResourceManager.GetSprite(texture));
                if (nightTexture != null && !loadedSprites.ContainsKey(backgroundNight))
                    loadedSprites.Add(backgroundNight, ResourceManager.GetSprite(nightTexture));
            }

            isTextureLoading = false;
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
            refreshed = false;
            map = null;
            isTextureLoading = false;

            TextureDestroy().Forget();

            return true;
        }

        protected override void OnDestroy()
        {
            if (Kernel.isPlaying)
                TextureDestroy().Forget();
        }

        async UniTaskVoid TextureDestroy()
        {
            await UniTask.WaitUntil(() => !isTextureLoading);

            foreach (var item in loadedSprites)
            {
                if (item.Value != null)
                {
                    Destroy(item.Value.texture);
                    Destroy(item.Value);
                }
            }

            loadedSprites.Clear();
        }
    }
}
