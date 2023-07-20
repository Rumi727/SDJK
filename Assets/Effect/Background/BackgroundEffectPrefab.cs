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
    [RequireComponent(typeof(RawImage)), RequireComponent(typeof(RawImageUVPos)), RequireComponent(typeof(CanvasGroup))]
    public sealed class BackgroundEffectPrefab : UIObjectPoolingBase
    {
        public RawImage rawImage => this.GetComponentFieldSave(_rawImage); [SerializeField] RawImage _rawImage;
        public RawImageUVPos rawImageUVPos => this.GetComponentFieldSave(_rawImageUVPos); [SerializeField] RawImageUVPos _rawImageUVPos;
        public CanvasGroup canvasGroup => this.GetComponentFieldSave(_canvasGroup); [SerializeField] CanvasGroup _canvasGroup;

        public EffectManager effectManager { get; private set; } = null;
        public MapFile map { get; private set; }

        public bool isRemoveQueue { get; set; } = false;

        public override void OnCreate()
        {
            base.OnCreate();
            transform.localScale = new Vector3(3, 3, 1);
        }

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
        Dictionary<string, Texture2D> loadedSprites = new Dictionary<string, Texture2D>();
        void Update()
        {
            BackgroundEffectPair background = map.globalEffect.background.GetValue(RhythmManager.currentBeatScreen);

            if (background.positionUnfreeze)
            {
                Vector2 pos;
                pos = effectManager.mainCamera.WorldToViewportPoint(Vector3.zero);

                pos.x *= canvas.pixelRect.width;
                pos.y *= canvas.pixelRect.height;

                pos -= new Vector2(canvas.pixelRect.width * 0.5f, canvas.pixelRect.height * 0.5f);
                pos /= canvas.scaleFactor;

                rawImageUVPos.position = pos;
            }
            else
                rawImageUVPos.position = Vector2.zero;

            if (background.rotationUnfreeze)
                rectTransform.localEulerAngles = new Vector3(0, 0, -effectManager.mainCamera.transform.localEulerAngles.z);
            else
                rectTransform.localEulerAngles = Vector2.zero;

            if (background.scaleUnfreeze)
                rectTransform.anchoredPosition3D = new Vector3(0, 0, -((effectManager.mainCamera.transform.position.z + CameraEffect.defaultDistance) / canvas.transform.localScale.z));
            else
                rectTransform.anchoredPosition3D = Vector3.zero;

            if (!isRemoveQueue && loadedSprites.Count > 0 && map == effectManager.selectedMap)
            {
                //1초동안 배경이 로딩되지 않았으면 알파값이 1이 될때까지 텍스쳐를 변경하지 않음
                if (timeoutTimer < 1 || (timeoutTimer >= 1 && canvasGroup.alpha >= 1))
                {
                    DateTime now = DateTime.Now;
                    string texturePath;
                    if (now.Hour >= 0 && now.Hour < 4)
                    {
                        texturePath = background.backgroundNightFile;

                        if (string.IsNullOrEmpty(texturePath))
                            texturePath = background.backgroundFile;
                    }
                    else
                        texturePath = background.backgroundFile;

                    if (texturePath != tempTexturePath)
                    {
                        tempTexturePath = texturePath;

                        textureChangeCancelSource.Cancel();
                        textureChangeCancelSource = new CancellationTokenSource();

                        TextureChange(texturePath).Forget();
                    }
                }
            }

            if (loadedSprites.Count > 0 && rawImage.texture != null)
            {
                canvasGroup.alpha = canvasGroup.alpha.MoveTowards(1, 0.05f * Kernel.fpsUnscaledSmoothDeltaTime);
                rawImage.color = map.globalEffect.backgroundColor.GetValue(RhythmManager.currentBeatScreen);
            }
            else
            {
                //배경이 로딩된 후 1초가 지나게 되면 알파값을 강제로 변경
                if (timeoutTimer >= 1)
                {
                    canvasGroup.alpha = canvasGroup.alpha.MoveTowards(1, 0.05f * Kernel.fpsUnscaledSmoothDeltaTime);
                    rawImage.color = Color.black;
                }
                else
                {
                    rawImage.color = Color.clear;
                    timeoutTimer += Kernel.unscaledDeltaTime;
                }
            }
        }

        async UniTaskVoid TextureLoad()
        {
            try
            {
                disableCreation = true;

                for (int i = 0; i < map.globalEffect.background.Count; i++)
                {
                    BackgroundEffectPair backgroundEffect = map.globalEffect.background[i].value;
                    string background = backgroundEffect.backgroundFile;
                    string backgroundNight = backgroundEffect.backgroundNightFile;

                    string texturePath = PathUtility.Combine(map.mapFilePathParent, background);
                    Texture2D texture = await ResourceManager.GetTextureAsync(texturePath, false, FilterMode.Bilinear, TextureWrapMode.Repeat, true, TextureMetaData.CompressionType.none);

                    if (texture != null && !loadedSprites.ContainsKey(background))
                        loadedSprites.Add(background, texture);

                    if (!Kernel.isPlaying || isRemoved || IsDestroyed())
                    {
                        TextureDestroy();
                        return;
                    }

                    string nightTexturePath = PathUtility.Combine(map.mapFilePathParent, backgroundNight);
                    Texture2D nightTexture = await ResourceManager.GetTextureAsync(nightTexturePath, false, FilterMode.Bilinear, TextureWrapMode.Repeat, true, TextureMetaData.CompressionType.none);

                    if (nightTexture != null && !loadedSprites.ContainsKey(backgroundNight))
                        loadedSprites.Add(backgroundNight, texture);

                    if (!Kernel.isPlaying || isRemoved || IsDestroyed())
                    {
                        TextureDestroy();
                        return;
                    }
                }
            }
            finally
            {
                disableCreation = false;
            }
        }

        CancellationTokenSource textureChangeCancelSource = new CancellationTokenSource();
        async UniTaskVoid TextureChange(string texturePath)
        {
            await UniTask.WaitUntil(() => loadedSprites.ContainsKey(texturePath), PlayerLoopTiming.Update, textureChangeCancelSource.Token);
            rawImage.texture = loadedSprites[texturePath];
        }

        public override void Remove()
        {
            base.Remove();

            tempTexturePath = "";
            rawImage.color = Color.black;
            canvasGroup.alpha = 0;
            refreshed = false;
            effectManager = null;
            isRemoveQueue = false;
            timeoutTimer = 0;

            rawImageUVPos.position = Vector2.zero;
            transform.localEulerAngles = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition3D = Vector3.zero;

            TextureDestroy();
        }

        protected override void OnDestroy() => TextureDestroy();

        void TextureDestroy()
        {
            foreach (var item in loadedSprites)
            {
                if (item.Value != null)
                {
                    if (Kernel.isPlaying)
                        Destroy(item.Value);
                    else
                        DestroyImmediate(item.Value);
                }
            }

            loadedSprites.Clear();

            if (Kernel.isPlaying)
                rawImage.texture = null;
        }
    }
}
