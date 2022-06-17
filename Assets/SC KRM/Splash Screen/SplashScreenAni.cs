using Cysharp.Threading.Tasks;
using SCKRM.Resource;
using UnityEngine;
using SCKRM.UI;
using TMPro;

namespace SCKRM.Splash
{
    [AddComponentMenu("SC KRM/Splash Screen/Splash Screen Ani")]
    public sealed class SplashScreenAni : Manager<SplashScreenAni>
    {
        [SerializeField] CanvasGroup progressBarCanvasGroup;
        [SerializeField] ProgressBar progressBar;
        [SerializeField] ProgressBar progressBarDetailed;

        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] Transform CSImage;
        [SerializeField] Transform CS;
        [SerializeField] TMP_Text text;
        [SerializeField] string showText = "";

        float xV;
        float yV;
        float rV;

        bool xFlip = false;

        AudioClip bow;
        AudioClip drawmap;

        async UniTaskVoid Awake()
        {
            if (SingletonCheck(this))
            {
                SplashScreen.isAniPlaying = true;

                canvasGroup.alpha = 0;
                progressBarCanvasGroup.alpha = 0;
                progressBar.allowNoResponse = false;

                bow = await ResourceManager.GetAudio(PathTool.Combine(Kernel.streamingAssetsPath, ResourceManager.soundPath.Replace("%NameSpace%", "minecraft"), "random/bow"));
                drawmap = await ResourceManager.GetAudio(PathTool.Combine(Kernel.streamingAssetsPath, ResourceManager.soundPath.Replace("%NameSpace%", "minecraft"), "ui/cartography_table/drawmap") + Random.Range(1, 4));

                if (await UniTask.DelayFrame(10, PlayerLoopTiming.Initialization, AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                    return;
                else if (!SplashScreen.isAniPlaying)
                    return;

                if (Random.Range(0, 2) == 1)
                    xFlip = true;
                else
                    xFlip = false;

                if (xFlip)
                {
                    CS.localPosition = new Vector2(670, Random.Range(-32f, 245f));
                    xV = Random.Range(-8f, -20f);
                    rV = Random.Range(10f, 20f);
                }
                else
                {
                    CS.localPosition = new Vector2(-670, Random.Range(-32f, 245f));
                    xV = Random.Range(8f, 20f);
                    rV = Random.Range(-10f, -20f);
                }

                yV = Random.Range(8f, 15f);

                //페이드 인
                while (canvasGroup.alpha < 1)
                {
                    canvasGroup.alpha += 0.05f * Kernel.fpsUnscaledDeltaTime;
                    if (await UniTask.DelayFrame(1, PlayerLoopTiming.Initialization, AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                        return;
                    else if (!SplashScreen.isAniPlaying)
                        return;
                }

                if (await UniTask.WaitUntil(() => InitialLoadManager.isSettingLoadEnd, PlayerLoopTiming.Initialization, AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                    return;
                else if (!SplashScreen.isAniPlaying)
                    return;

                canvasGroup.alpha = 1;
                AudioSource.PlayClipAtPoint(bow, Vector3.zero);

                //C# 던지기
                while (!((CS.localPosition.x >= -75 && CS.localPosition.x <= 75 && CS.localPosition.y >= -75 && CS.localPosition.y <= 75) || (xFlip && (CS.localPosition.x <= -500 || CS.localPosition.y <= -300)) || (!xFlip && (CS.localPosition.x >= 500 || CS.localPosition.y <= -300))))
                {
                    CS.localPosition = new Vector2(CS.localPosition.x + xV * Kernel.fpsUnscaledDeltaTime, CS.localPosition.y + yV * Kernel.fpsUnscaledDeltaTime);
                    CSImage.transform.localEulerAngles = new Vector3(CSImage.transform.localEulerAngles.x, CSImage.transform.localEulerAngles.y, CSImage.transform.localEulerAngles.z + rV * Kernel.fpsUnscaledDeltaTime);
                    yV -= 0.5f * Kernel.fpsUnscaledDeltaTime;

                    if (SplashScreen.SaveData.allowProgressBarShow)
                        progressBarCanvasGroup.alpha += 0.05f * Kernel.fpsUnscaledDeltaTime;

                    if (await UniTask.DelayFrame(1, PlayerLoopTiming.Initialization, AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                        return;
                    else if (!SplashScreen.isAniPlaying)
                        return;
                }

                text.rectTransform.anchoredPosition = new Vector3(0, -13);
                text.text = showText;

                AudioSource.PlayClipAtPoint(drawmap, Vector3.zero);

                //페이드 아웃
                {
                    float timer = 0;
                    while (canvasGroup.alpha > 0)
                    {
                        text.rectTransform.anchoredPosition = text.rectTransform.anchoredPosition.Lerp(Vector3.zero, 0.1f * Kernel.fpsUnscaledDeltaTime);
                        CSImage.transform.rotation = Quaternion.Lerp(CSImage.transform.rotation, Quaternion.Euler(Vector3.zero), 0.1f * Kernel.fpsUnscaledDeltaTime);
                        CS.localPosition = CS.localPosition.Lerp(new Vector3(24, -24), 0.1f * Kernel.fpsUnscaledDeltaTime);

                        if (timer >= 2 && InitialLoadManager.isInitialLoadEnd)
                            canvasGroup.alpha -= 0.05f * Kernel.fpsUnscaledDeltaTime;
                        else
                        {
                            if (timer >= 3)
                            {
                                text.text = "Please wait...";
                                progressBarCanvasGroup.alpha += 0.05f * Kernel.fpsUnscaledDeltaTime;
                            }
                            else
                                timer += Kernel.unscaledDeltaTime;
                        }

                        if (SplashScreen.SaveData.allowProgressBarShow)
                            progressBarCanvasGroup.alpha += 0.05f * Kernel.fpsUnscaledDeltaTime;

                        if (await UniTask.DelayFrame(1, PlayerLoopTiming.Initialization, AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                            return;
                        else if (!SplashScreen.isAniPlaying)
                            return;
                    }
                }

                SplashScreen.isAniPlaying = false;
            }
        }

        void Update()
        {
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
        }
    }
}