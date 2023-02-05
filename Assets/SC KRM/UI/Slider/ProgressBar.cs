using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.UI
{
    [AddComponentMenu("SC KRM/UI/Slider/Progress Bar")]
    public sealed class ProgressBar : UIAni
    {
        [SerializeField, Min(0)] float _progress; public float progress { get => _progress; set => _progress = value.Clamp(0); }
        [SerializeField, Min(0)] float _maxProgress; public float maxProgress { get => _maxProgress; set => _maxProgress = value.Clamp(0); }



        [SerializeField, NotNull] Slider _slider;
        public Slider slider => _slider;

        [SerializeField, NotNull] RectTransform _fillShow;
        public RectTransform fillShow => _fillShow;


        [SerializeField] bool _allowNoResponse = true; public bool allowNoResponse { get => _allowNoResponse; set => _allowNoResponse = value; }
        public bool isNoResponse { get; private set; } = false;

        [System.NonSerialized] float loopValue = 0;
        [System.NonSerialized] float tempProgress = 0;
        [System.NonSerialized] float tempTimer = 0;
        [System.NonSerialized] float tempMinX = 0;
        [System.NonSerialized] float tempMaxX = 0;



        void Update()
        {
            if (tempTimer >= 1 && allowNoResponse && progress < maxProgress)
            {
                if (slider.enabled)
                    slider.enabled = false;

                if (!isNoResponse)
                {
                    tempMinX = fillShow.anchorMin.x - (loopValue - 0.25f).Clamp01();
                    tempMaxX = fillShow.anchorMax.x - loopValue.Clamp01();

                    isNoResponse = true;
                }

                loopValue += 0.0125f * Kernel.fpsUnscaledSmoothDeltaTime;

                tempMinX = tempMinX.Lerp(0, 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
                tempMaxX = tempMaxX.Lerp(0, 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);

                fillShow.anchorMin = new Vector2((loopValue - 0.25f + tempMinX).Clamp01(), fillShow.anchorMin.y);
                fillShow.anchorMax = new Vector2((loopValue + tempMaxX).Clamp01(), fillShow.anchorMax.y);

                if (fillShow.anchorMin.x >= 1)
                    loopValue = 0;
            }
            else
            {
                if (!slider.enabled)
                    slider.enabled = true;

                isNoResponse = false;

                slider.value = progress;
                slider.maxValue = maxProgress;

                fillShow.anchorMin = fillShow.anchorMin.Lerp(slider.fillRect.anchorMin, 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
                fillShow.anchorMax = fillShow.anchorMax.Lerp(slider.fillRect.anchorMax, 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);

                tempTimer += Kernel.unscaledDeltaTime;
            }

            if (tempProgress != progress)
            {
                tempTimer = 0;
                tempProgress = progress;
            }
        }

        public void Initialize()
        {
            loopValue = 0;
            isNoResponse = false;
            tempProgress = 0;
            tempTimer = 0;
            tempMinX = 0;
            tempMaxX = 0;

            slider.value = 0;
            fillShow.anchorMin = new Vector2(0, slider.fillRect.anchorMin.y);
            fillShow.anchorMax = new Vector2(0, slider.fillRect.anchorMax.y);
        }
    }
}
