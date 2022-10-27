using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Resource;
using SCKRM.Rhythm;
using SCKRM.Sound;
using SCKRM.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace SDJK
{
    [RequireComponent(typeof(VideoPlayer)), RequireComponent(typeof(CanvasGroup))]
    public sealed class VideoEffectPrefab : UIObjectPooling
    {
        public VideoPlayer videoPlayer => this.GetComponentFieldSave(_videoPlayer); [SerializeField] VideoPlayer _videoPlayer;
        public CanvasGroup canvasGroup => this.GetComponentFieldSave(_canvasGroup); [SerializeField] CanvasGroup _canvasGroup;

        public RawImage rawImage => _rawImage; [SerializeField, NotNull] RawImage _rawImage;
        public AspectRatioFitter aspectRatioFitter => _aspectRatioFitter; [SerializeField, NotNull] AspectRatioFitter _aspectRatioFitter;



        public RenderTexture renderTexture { get; private set; } = null;



        protected override void Awake()
        {
            renderTexture = new RenderTexture(1, 1, 24);

            rawImage.texture = renderTexture;
            videoPlayer.targetTexture = renderTexture;
        }

        Map.Map map;
        public override void OnCreate()
        {
            base.OnCreate();

            isPadeOut = false;

            rectTransform.sizeDelta = Vector2.zero;
            transform.SetSiblingIndex(0);
        }

        bool refreshed = false;
        public void Refresh(Map.Map map, ISoundPlayer soundPlayer)
        {
            if (refreshed)
            {
                Remove();
                return;
            }

            refreshed = true;
            this.map = map;
            this.soundPlayer = soundPlayer;

            string videoPath = PathTool.Combine(map.mapFilePathParent, map.info.videoBackgroundFile);
            if (ResourceManager.FileExtensionExists(videoPath, out string fullPath, ResourceManager.videoExtension))
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
        ISoundPlayer soundPlayer = null;
        void Update()
        {
            if (isPlaying && !videoPlayer.isPlaying && !videoPlayer.isPaused)
            {
                PadeOut().Forget();
                return;
            }

            if (!isPadeOut && videoPlayer.isPrepared && videoPlayer.canStep)
            {
                isPlaying = true;

                if (videoPlayer.width != renderTexture.width || videoPlayer.height != renderTexture.height)
                    SetResolution();

                rawImage.color = map.globalEffect.videoColor.GetValue(RhythmManager.currentBeatScreen);

                if (soundPlayer != null)
                {
                    double dis = (soundPlayer.time + offset) - videoPlayer.time;
                    float speed = soundPlayer.speed * Kernel.gameSpeed;
                    videoPlayer.playbackSpeed = speed;

                    if (dis.Abs() < 1)
                    {
                        canvasGroup.alpha = canvasGroup.alpha.MoveTowards(1, 0.05f * Kernel.fpsUnscaledDeltaTime);

                        if (dis >= 0.06)
                            videoPlayer.playbackSpeed = speed * 4;

                        if (dis <= -0.06)
                            videoPlayer.playbackSpeed = speed * 0.25f;
                    }
                    else
                        videoPlayer.time = RhythmManager.time + offset;

                    if (videoPlayer.isPaused != soundPlayer.isPaused)
                    {
                        if (RhythmManager.soundPlayer.isPaused)
                            videoPlayer.Pause();
                        else
                            videoPlayer.Play();
                    }
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
                canvasGroup.alpha = canvasGroup.alpha.MoveTowards(0, 0.05f * Kernel.fpsUnscaledDeltaTime);
                await UniTask.NextFrame();

                if (this == null)
                    return;
            }

            Remove();
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            isPadeOut = false;
            isPlaying = false;
            offset = 0;

            videoPlayer.Stop();
            renderTexture.Release();
            canvasGroup.alpha = 0;

            refreshed = false;
            return true;
        }
    }
}