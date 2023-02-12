using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SCKRM
{
    [WikiDescription("SC KRM의 초기 로딩이 끝난 직후 페이드 아웃을 표시하기 위한 클래스 입니다")]
    [AddComponentMenu("SC KRM/Kernel/Splash Screen Background")]
    public sealed class SplashScreenBackground : MonoBehaviour
    {
        [SerializeField] Graphic graphic;
        void OnEnable() => InitialLoadManager.initialLoadEndSceneMove += PadeOut;
        void OnDisable() => InitialLoadManager.initialLoadEndSceneMove -= PadeOut;

        async void PadeOut()
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 1);

            if (graphic != null)
            {
                while (graphic.color.a > 0)
                {
                    Color color = graphic.color;
                    graphic.color = new Color(color.r, color.g, color.b, color.a.MoveTowards(0, 0.05f * Kernel.fpsUnscaledSmoothDeltaTime));

                    if (await UniTask.DelayFrame(1, PlayerLoopTiming.Initialization, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow())
                        return;
                }

                graphic.gameObject.SetActive(false);
            }
        }
    }
}
