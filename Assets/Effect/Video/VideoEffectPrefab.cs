using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Resource;
using SCKRM.Rhythm;
using SCKRM.Sound;
using SCKRM.UI;
using SDJK.Map;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace SDJK.Effect
{
    [RequireComponent(typeof(VideoPlayer)), RequireComponent(typeof(CanvasGroup))]
    public sealed class VideoEffectPrefab : UIObjectPoolingBase
    {
        public VideoPlayer videoPlayer => this.GetComponentFieldSave(_videoPlayer); [SerializeField] VideoPlayer _videoPlayer;
        public CanvasGroup canvasGroup => this.GetComponentFieldSave(_canvasGroup); [SerializeField] CanvasGroup _canvasGroup;

        public RawImage rawImage => _rawImage; [SerializeField, FieldNotNull] RawImage _rawImage;
        public AspectRatioFitter aspectRatioFitter => _aspectRatioFitter; [SerializeField, FieldNotNull] AspectRatioFitter _aspectRatioFitter;



        public RenderTexture renderTexture { get; private set; } = null;

        public EffectManager effectManager { get; private set; } = null;

        public MapPack mapPack { get; private set; } = null;
        public MapFile map { get; private set; } = null;



        protected override void Awake()
        {
            renderTexture = new RenderTexture(1, 1, 24);

            rawImage.texture = renderTexture;
            videoPlayer.targetTexture = renderTexture;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            isPadeOut = false;

            rectTransform.sizeDelta = Vector2.zero;
            transform.SetSiblingIndex(0);
        }

        bool refreshed = false;
        public void Refresh(EffectManager effectManager)
        {
            if (refreshed)
            {
                Remove();
                return;
            }

            refreshed = true;
            this.effectManager = effectManager;

            mapPack = effectManager.selectedMapPack;
            map = effectManager.selectedMap;

            DateTime now = DateTime.Now;
            string videoPath;
            string fullPath;
            if (now.Hour >= 0 && now.Hour < 4)
            {
                videoPath = PathUtility.Combine(map.mapFilePathParent, map.info.videoBackgroundNightFile);
                if (!ResourceManager.FileExtensionExists(videoPath, out fullPath, ResourceManager.videoExtension))
                {
                    videoPath = PathUtility.Combine(map.mapFilePathParent, map.info.videoBackgroundFile);
                    if (!ResourceManager.FileExtensionExists(videoPath, out fullPath, ResourceManager.videoExtension))
                        videoPath = "";
                }
            }
            else
            {
                videoPath = PathUtility.Combine(map.mapFilePathParent, map.info.videoBackgroundFile);
                if (!ResourceManager.FileExtensionExists(videoPath, out fullPath, ResourceManager.videoExtension))
                    videoPath = "";
            }

            if (videoPath != "")
            {
                offset = map.info.videoOffset;

                videoPlayer.url = fullPath;
                videoPlayer.time = RhythmManager.time + offset;

                videoPlayer.Prepare();
                videoPlayer.Play();

                SetResolution();
            }
            else
                Remove();
        }

        double offset = 0;
        bool isPlaying = false;
        void Update()
        {
            if (isPlaying && !videoPlayer.isPlaying && !videoPlayer.isPaused)
            {
                if (!isPadeOut)
                    PadeOut().Forget();

                return;
            }

            if (!isPadeOut &&
                videoPlayer.isPrepared &&
                videoPlayer.canStep &&
                mapPack == effectManager.selectedMapPack &&
                map.info.videoBackgroundFile == effectManager.selectedMap.info.videoBackgroundFile &&
                map.info.videoBackgroundNightFile == effectManager.selectedMap.info.videoBackgroundNightFile &&
                map.info.videoOffset == effectManager.selectedMap.info.videoOffset)
            {
                isPlaying = true;

                if (videoPlayer.width != renderTexture.width || videoPlayer.height != renderTexture.height)
                    SetResolution();

                rawImage.color = map.globalEffect.backgroundEffect.videoColor.GetValue(RhythmManager.currentBeatScreen);

                double time = RhythmManager.internalTime + offset;
                if (time >= 0 && time < videoPlayer.length - 0.1f)
                {
                    double dis = time - videoPlayer.time;
                    float speed = (float)(RhythmManager.speed * Kernel.gameSpeed);

                    if (!videoPlayer.isPaused)
                        videoPlayer.playbackSpeed = speed;

                    if (dis.Abs() < 1)
                    {
                        canvasGroup.alpha = canvasGroup.alpha.MoveTowards(1, 0.05f * Kernel.fpsUnscaledSmoothDeltaTime);

                        if (!videoPlayer.isPaused)
                        {
                            if (dis >= 0.015625)
                                videoPlayer.playbackSpeed = speed * 4;

                            if (dis <= -0.015625)
                                videoPlayer.playbackSpeed = speed * 0.25f;
                        }
                    }
                    else if (!videoPlayer.isPaused)
                        videoPlayer.time = time;

                    if (videoPlayer.isPaused != (RhythmManager.isPaused && RhythmManager.time >= 0))
                    {
                        if (RhythmManager.isPaused && RhythmManager.time >= 0)
                            videoPlayer.Pause();
                        else
                            videoPlayer.Play();
                    }
                }
                else
                {
                    if (!videoPlayer.isPaused)
                        videoPlayer.Pause();

                    canvasGroup.alpha = canvasGroup.alpha.MoveTowards(0, 0.05f * Kernel.fpsUnscaledSmoothDeltaTime);
                }
            }
        }

        void SetResolution()
        {
            renderTexture.width = (int)videoPlayer.width;
            renderTexture.height = (int)videoPlayer.height;

            aspectRatioFitter.aspectRatio = (float)videoPlayer.width / videoPlayer.height;
        }

        bool isPadeOut = false;
        public async UniTaskVoid PadeOut()
        {
            isPadeOut = true;

            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha = canvasGroup.alpha.MoveTowards(0, 0.05f * Kernel.fpsUnscaledSmoothDeltaTime);
                await UniTask.NextFrame();

                if (this == null)
                    return;
            }

            Remove();
        }

        public override void Remove()
        {
            base.Remove();

            isPadeOut = false;
            isPlaying = false;
            offset = 0;

            videoPlayer.Stop();
            renderTexture.Release();
            canvasGroup.alpha = 0;

            refreshed = false;
        }
    }
}
