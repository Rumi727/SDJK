#nullable enable
using Cysharp.Threading.Tasks;
using SCKRM.Resource;
using SCKRM.UI;
using TMPro;
using UnityEngine;

namespace SCKRM.Splash
{
    public sealed class DefaultSplashAni : MonoBehaviour
    {
        [SerializeField, FieldNotNull] CanvasGroup? progressBarCanvasGroup;
        [SerializeField, FieldNotNull] ProgressBar? progressBar;
        [SerializeField, FieldNotNull] ProgressBar? progressBarDetailed;

        [SerializeField, FieldNotNull] TMP_Text? text;

        [SerializeField, FieldNotNull] Animator? logo;

        [SerializeField] int mainLayer = 0;
        [SerializeField] string startParameter = "Start";
        [SerializeField] string endParameter = "End";

        int startHash;
        int endHash;
        void OnEnable()
        {
            startHash = Animator.StringToHash(startParameter);
            endHash = Animator.StringToHash(endParameter);
        }

        float timer = 0;
        async UniTaskVoid Start()
        {
            if (logo == null || progressBar == null || progressBarDetailed == null || progressBarCanvasGroup == null || text == null)
                return;

            await UniTask.Delay(1000);

            while (true)
            {
                AnimatorStateInfo currentState = logo.GetCurrentAnimatorStateInfo(mainLayer);

                bool start = logo.GetBool(startHash);
                bool end = logo.GetBool(endHash);

                if (SplashScreen.isAniPlaying)
                {
                    if (start && currentState.normalizedTime >= 1 && InitialLoadManager.isInitialLoadEnd)
                    {
                        logo.SetBool(startHash, false);
                        logo.SetBool(endHash, true);
                    }
                    else if (end && currentState.normalizedTime >= 1)
                    {
                        SplashScreen.isAniPlaying = false;

                        logo.SetBool(startHash, false);
                        logo.SetBool(endHash, false);
                    }
                    else if (!start && !end)
                        logo.SetBool(startHash, true);
                }
                else if (start || end)
                {
                    logo.SetBool(startHash, false);
                    logo.SetBool(endHash, false);
                }

                if (SplashScreen.SaveData.allowProgressBarShow)
                    progressBarCanvasGroup.alpha += 0.05f * Kernel.fpsUnscaledSmoothDeltaTime;
                else
                {
                    if (timer >= 5)
                    {
                        text.text = "Please wait...";
                        progressBarCanvasGroup.alpha += 0.05f * Kernel.fpsUnscaledSmoothDeltaTime;
                    }
                    else
                        timer += Kernel.unscaledDeltaTime;
                }

                if (ResourceManager.resourceRefreshAsyncTask != null)
                {
                    progressBar.progress = ResourceManager.resourceRefreshAsyncTask.progress;
                    progressBar.maxProgress = ResourceManager.resourceRefreshAsyncTask.maxProgress;
                }
                else
                {
                    progressBar.progress = 1;
                    progressBar.maxProgress = 1;
                }

                if (ResourceManager.resourceRefreshDetailedAsyncTask != null)
                {
                    progressBarDetailed.progress = ResourceManager.resourceRefreshDetailedAsyncTask.progress;
                    progressBarDetailed.maxProgress = ResourceManager.resourceRefreshDetailedAsyncTask.maxProgress;
                }
                else
                {
                    progressBarDetailed.progress = 1;
                    progressBarDetailed.maxProgress = 1;
                }

                if (await UniTask.NextFrame(this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow())
                    return;
            }
        }
    }
}
