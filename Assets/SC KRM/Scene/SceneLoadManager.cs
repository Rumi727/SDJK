using Cysharp.Threading.Tasks;
using SCKRM.Input;
using SCKRM.UI.StatusBar;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SCKRM
{
    public sealed class SceneLoadManager : Manager<SceneLoadManager>
    {
        [SerializeField, NotNull] GameObject _sceneLoadingAni; public GameObject sceneLoadingAni => _sceneLoadingAni;
        [SerializeField, NotNull] GameObject _background; public GameObject background => _background;
        [SerializeField, NotNull] RectTransform _aniImage; public RectTransform aniImage => _aniImage;

        public static bool isDone { get; private set; } = true;
        public static float progress { get; private set; } = 0;
        public static bool isLoading { get; private set; } = false;
        public static bool allowSceneActivation { get; set; } = false;

        void Awake() => SingletonCheck(this);

        public static async UniTask LoadScene(string sceneName) => await loadScene(SceneManager.GetSceneByName(sceneName).buildIndex);
        public static async UniTask LoadScene(int sceneBuildIndex) => await loadScene(sceneBuildIndex);

        static CancellationTokenSource cancelSource;
        static async UniTask loadScene(int sceneBuildIndex)
        {
            if (isLoading)
            {
                Debug.LogWarning("Could not load another scene while loading scene");
                return;
            }



            isLoading = true;
            isDone = false;
            progress = 0;
            allowSceneActivation = false;

            cancelSource?.Cancel();
            cancelSource = new CancellationTokenSource();

            instance.aniImage.anchorMin = Vector2.zero;
            instance.aniImage.anchorMax = Vector2.up;

            instance.sceneLoadingAni.SetActive(true);

#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
            InputManager.sceneInputLock = true;
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.
            instance.background.SetActive(true);

            //Start Ani
            {
                while (instance.aniImage.anchorMax.x <= 0.999f)
                {
                    instance.aniImage.anchorMax = instance.aniImage.anchorMax.Lerp(Vector2.one, 0.2f * Kernel.fpsDeltaTime);
                    await UniTask.NextFrame();
                }

                instance.aniImage.anchorMax = Vector2.one;
            }

            //Loading
            {
                SceneManager.LoadScene(1);
                await UniTask.NextFrame();

                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex);
                asyncOperation.allowSceneActivation = false;

                while (!asyncOperation.isDone)
                {
                    progress = asyncOperation.progress;
                    asyncOperation.allowSceneActivation = allowSceneActivation;

                    await UniTask.NextFrame(PlayerLoopTiming.Initialization);
                }

#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
                InputManager.sceneInputLock = false;
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.
                instance.background.SetActive(false);

                isLoading = false;
            }

            //End Ani
            {
                while (instance.aniImage.anchorMin.x <= 0.999f)
                {
                    instance.aniImage.anchorMin = instance.aniImage.anchorMin.Lerp(Vector2.right, 0.2f * Kernel.fpsDeltaTime);

                    if (await UniTask.NextFrame(cancelSource.Token).SuppressCancellationThrow())
                        return;
                }

                instance.aniImage.anchorMin = Vector2.zero;
                instance.aniImage.anchorMax = Vector2.up;
            }

            instance.sceneLoadingAni.SetActive(false);
        }
    }
}
