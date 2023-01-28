#if !UNITY_EDITOR
using Cysharp.Threading.Tasks;
using SCKRM.Input;
using UnityEngine;
#endif

namespace SCKRM
{
    public static class RuntimeFullScreen
    {
        #if !UNITY_EDITOR
        [Starten]
        static async UniTaskVoid Starten()
        {
            while (true)
            {
                if (InitialLoadManager.isInitialLoadEnd && InputManager.GetKey("kernel.full_screen", InputType.Down, InputManager.inputLockDenyAllForce))
                {
                    if (Screen.fullScreen)
                        Screen.SetResolution((int)(ScreenManager.currentResolution.width / 1.5f), (int)(ScreenManager.currentResolution.height / 1.5f), false);
                    else
                    {
                        Screen.SetResolution(ScreenManager.currentResolution.width, ScreenManager.currentResolution.height, false);

                        if (await UniTask.DelayFrame(4, PlayerLoopTiming.LastPostLateUpdate, AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                            return;

                        Screen.SetResolution(ScreenManager.currentResolution.width, ScreenManager.currentResolution.height, true);
                    }
                }

                if (await UniTask.NextFrame(PlayerLoopTiming.Update, AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                    return;
            }
        }
#endif
    }
}