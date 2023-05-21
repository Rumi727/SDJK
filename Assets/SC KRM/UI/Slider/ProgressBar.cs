using UnityEngine;

namespace SCKRM.UI
{
    [AddComponentMenu("SC KRM/UI/Slider/Progress Bar"), ExecuteAlways]
    public sealed class ProgressBar : UIAniBase
    {
        [SerializeField, Min(0)] float _progress; public float progress { get => _progress; set => _progress = value.Clamp(0); }
        [SerializeField, Min(0)] float _maxProgress; public float maxProgress { get => _maxProgress; set => _maxProgress = value.Clamp(0); }



        [SerializeField, FieldNotNull] RectTransform _fillArea;
        public RectTransform fillArea => _fillArea;

        [SerializeField, FieldNotNull] RectTransform _fill;
        public RectTransform fill => _fill;

        [SerializeField, FieldNotNull] SlicedFilledImage _fillSlicedFilledImage;
        public SlicedFilledImage fillSlicedFilledImage => _fillSlicedFilledImage;

        [SerializeField] bool _right = false;
        public bool right { get => _right; set => _right = value; }


        [SerializeField] bool _allowNoResponse = true; public bool allowNoResponse { get => _allowNoResponse; set => _allowNoResponse = value; }
        public bool isNoResponse { get; private set; } = false;

        [System.NonSerialized] float loopValue = 0;
        [System.NonSerialized] float tempProgress = 0;
        [System.NonSerialized] float tempTimer = 0;
        [System.NonSerialized] float tempMinX = 0;
        [System.NonSerialized] float tempMaxX = 0;


        
        protected override void Awake() => Initialize();



        [System.NonSerialized] float anchorMinX = 0;
        [System.NonSerialized] float anchorMaxX = 0;
        void Update()
        {
            float lerpValue;
            if (Kernel.isPlaying && lerp)
                lerpValue = this.lerpValue * Kernel.fpsUnscaledSmoothDeltaTime;
            else
                lerpValue = 1;

            if (Kernel.isPlaying && tempTimer >= 1 && allowNoResponse && progress < maxProgress)
            {
                if (!isNoResponse)
                {
                    if (right)
                    {
                        tempMinX = anchorMinX - loopValue.Clamp01();
                        tempMaxX = anchorMaxX - (loopValue + 0.25f).Clamp01();
                    }
                    else
                    {
                        tempMinX = anchorMinX - (loopValue - 0.25f).Clamp01();
                        tempMaxX = anchorMaxX - loopValue.Clamp01();
                    }

                    isNoResponse = true;
                }

                if (right)
                    loopValue -= 0.0125f * Kernel.fpsUnscaledSmoothDeltaTime;
                else
                    loopValue += 0.0125f * Kernel.fpsUnscaledSmoothDeltaTime;

                tempMinX = tempMinX.Lerp(0, lerpValue);
                tempMaxX = tempMaxX.Lerp(0, lerpValue);

                if (right)
                {
                    anchorMinX = (loopValue + tempMinX).Clamp01();
                    anchorMaxX = (loopValue + 0.25f + tempMaxX).Clamp01();
                }
                else
                {
                    anchorMinX = (loopValue - 0.25f + tempMinX).Clamp01();
                    anchorMaxX = (loopValue + tempMaxX).Clamp01();
                }

                if (right)
                {
                    if (anchorMaxX <= 0)
                        loopValue = 1;
                }
                else
                {
                    if (anchorMinX >= 1)
                        loopValue = 0;
                }
            }
            else
            {
                if (Kernel.isPlaying)
                {
                    isNoResponse = false;
                    tempTimer += Kernel.unscaledDeltaTime;
                }

                float temp = progress / maxProgress;
                if (!float.IsNormal(temp))
                    temp = 0;

                if (right)
                {
                    anchorMinX = anchorMinX.Lerp(1 - temp, lerpValue);
                    anchorMaxX = anchorMaxX.Lerp(1, lerpValue);
                }
                else
                {
                    anchorMinX = anchorMinX.Lerp(0, lerpValue);
                    anchorMaxX = anchorMaxX.Lerp(temp, lerpValue);
                }
            }

            {
                float clampMin = 1 - fill.rect.height / fillArea.rect.width;
                float clampMax = fill.rect.height / fillArea.rect.width;

                fill.anchorMin = new Vector2(anchorMinX.Clamp(0, clampMin), fill.anchorMin.y);
                fill.anchorMax = new Vector2(anchorMaxX.Clamp(clampMax, 1), fill.anchorMax.y);

                if (anchorMaxX <= clampMax)
                {
                    if (fillSlicedFilledImage.fillDirection != SlicedFilledImage.FillDirection.Right)
                        fillSlicedFilledImage.fillDirection = SlicedFilledImage.FillDirection.Right;

                    float fillAmount = 0f.InverseLerp(clampMax, anchorMaxX);
                    if (fillSlicedFilledImage.fillAmount != fillAmount)
                        fillSlicedFilledImage.fillAmount = fillAmount;
                }
                else if (anchorMinX >= clampMin)
                {
                    if (fillSlicedFilledImage.fillDirection != SlicedFilledImage.FillDirection.Left)
                        fillSlicedFilledImage.fillDirection = SlicedFilledImage.FillDirection.Left;

                    float fillAmount = 1 - clampMin.InverseLerp(1f, anchorMinX);
                    if (fillSlicedFilledImage.fillAmount != fillAmount)
                        fillSlicedFilledImage.fillAmount = fillAmount;
                }
                else
                {
                    if (fillSlicedFilledImage.fillAmount != 1)
                        fillSlicedFilledImage.fillAmount = 1;
                }
            }

            if (Kernel.isPlaying && tempProgress != progress)
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

            anchorMinX = 0;
            anchorMaxX = 0;

            fill.anchorMin = new Vector2(0, fill.anchorMin.y);
            fill.anchorMax = new Vector2(0, fill.anchorMax.y);

            fillSlicedFilledImage.fillAmount = 1;
        }
    }
}