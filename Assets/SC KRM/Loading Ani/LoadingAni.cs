using SCKRM.Input;
using SCKRM.Object;
using SCKRM.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCKRM.Loading
{
    public sealed class LoadingAni : ObjectPoolingBase
    {
        public float progress { get; set; } = 0;
        public float maxProgress { get; set; } = 1;

        public bool startAni { get; set; } = true;
        public bool endAni { get; set; } = true;

        public bool isStartAniEnd { get; private set; } = false;
        public bool isLoadingEnd { get; private set; } = false;

        public bool isLongLoadingAniEnd { get; private set; } = true;

        [SerializeField, FieldNotNull] GameObject background;
        [SerializeField, FieldNotNull] RectTransform aniImage;
        [SerializeField, FieldNotNull] CanvasGroup canvasGroup;
        [SerializeField, FieldNotNull] ProgressBar progressBar;

        public event Action startAniEndAction;
        public event Action endAniEndAction;

        public event Action loadingEndAction;

        void OnEnable() => DontDestroyOnLoad(this);

        float timer = 0;
        float alpha = 0;
        void Update()
        {
            progressBar.maxProgress = maxProgress;

            if (!isStartAniEnd || progress < maxProgress)
            {
                if (startAni)
                    aniImage.anchorMax = aniImage.anchorMax.Lerp(Vector2.one, LoadingAniManager.Data.aniLerp * Kernel.fpsUnscaledSmoothDeltaTime);
                else
                    aniImage.anchorMax = Vector2.one;

                progressBar.progress = progress;

                if (aniImage.anchorMax.x > 0.999f)
                {
                    if (!isStartAniEnd)
                    {
                        aniImage.anchorMax = Vector2.one;

                        startAniEndAction?.Invoke();
                        isStartAniEnd = true;
                    }

                    if (timer >= LoadingAniManager.Data.longLoadingTime)
                    {
                        alpha = alpha.MoveTowards(1, 0.01f * Kernel.fpsUnscaledSmoothDeltaTime);
                        canvasGroup.alpha = alpha;

                        isLongLoadingAniEnd = false;
                    }
                    else
                    {
                        timer += Kernel.unscaledDeltaTime;
                        isLongLoadingAniEnd = true;
                    }
                }

            }
            else
            {
                if (!isLoadingEnd)
                {
                    background.SetActive(false);
                    LoadingAniManager.loadingAnis.Remove(this);

                    loadingEndAction?.Invoke();
                    isLoadingEnd = true;
                }

                alpha = alpha.MoveTowards(-0.1f, 0.01f * Kernel.fpsUnscaledSmoothDeltaTime);
                canvasGroup.alpha = alpha;

                progressBar.progress = maxProgress;

                if (alpha <= -0.1f)
                {
                    isLongLoadingAniEnd = true;

                    if (endAni)
                        aniImage.anchorMin = aniImage.anchorMin.Lerp(Vector2.right, LoadingAniManager.Data.aniLerp * Kernel.fpsUnscaledSmoothDeltaTime);
                    else
                        aniImage.anchorMin = Vector2.right;

                    if (aniImage.anchorMin.x > 0.999f)
                    {
                        endAniEndAction?.Invoke();
                        Remove();
                    }
                }
            }
        }

        public override void Remove()
        {
            base.Remove();

            timer = 0;
            alpha = 0;

            progress = 0;
            maxProgress = 1;

            progressBar.progress = 0;
            progressBar.maxProgress = 1;

            startAni = true;
            endAni = true;

            isStartAniEnd = false;
            isLoadingEnd = false;

            isLongLoadingAniEnd = true;

            aniImage.anchorMin = Vector2.zero;
            aniImage.anchorMax = Vector2.up;

            startAniEndAction = null;
            endAniEndAction = null;
            loadingEndAction = null;

            background.SetActive(true);
            canvasGroup.alpha = 0;

            LoadingAniManager.loadingAnis.Remove(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            LoadingAniManager.loadingAnis.Remove(this);
        }
    }
}
