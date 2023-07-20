using Cysharp.Threading.Tasks;
using SCKRM.Loading;
using System;
using UnityEngine;

namespace SCKRM.Scene
{
    public static class SceneManager
    {
        public static bool isDone { get; private set; } = true;
        public static bool isLoading { get; private set; } = false;

        public static event Action activeSceneChanged;

        public static UniTask LoadScene(int sceneBuildIndex, Func<UniTask> loadingDelay = null)
        {
            if (isLoading)
            {
                Debug.LogWarning("Could not load another scene while loading scene");
                return UniTask.CompletedTask;
            }

            return loadScene(sceneBuildIndex, loadingDelay);
        }

        static async UniTask loadScene(int sceneBuildIndex, Func<UniTask> loadingDelay)
        {
            isLoading = true;
            isDone = false;

            try
            {
                LoadingAni loadingAni = LoadingAniManager.LoadingStart();

                loadingAni.progress = 0;
                loadingAni.maxProgress = 1;

                await UniTask.WaitUntil(() => loadingAni.isStartAniEnd || loadingAni.isRemoved);

                try
                {
                    activeSceneChanged?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
                await UniTask.NextFrame();

                AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneBuildIndex);
                asyncOperation.allowSceneActivation = false;

                if (loadingDelay != null)
                    await loadingDelay.Invoke();

                while (!asyncOperation.isDone || !asyncOperation.allowSceneActivation)
                {
                    if (asyncOperation.progress >= 0.89f)
                        loadingAni.progress = 1;
                    else
                        loadingAni.progress = asyncOperation.progress;

                    asyncOperation.allowSceneActivation = loadingAni.isLongLoadingAniEnd && asyncOperation.progress >= 0.89f;
                    await UniTask.NextFrame(PlayerLoopTiming.Initialization);
                }

                loadingAni.progress = 1;
            }
            finally
            {
                isLoading = false;
                isDone = true;
            }
        }
    }
}
