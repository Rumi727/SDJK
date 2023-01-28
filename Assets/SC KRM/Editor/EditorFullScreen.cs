using Cysharp.Threading.Tasks;
using SCKRM.Editor;
using SCKRM.Input;

namespace SCKRM
{
    public static class EditorFullScreen
    {
        [Starten]
        static async UniTaskVoid Starten()
        {
            while (true)
            {
                if (InitialLoadManager.isInitialLoadEnd && InputManager.GetKey("kernel.full_screen", InputType.Down, InputManager.inputLockDenyAllForce))
                    EditorTool.gameView.maximized = !EditorTool.gameView.maximized;

                if (await UniTask.NextFrame(PlayerLoopTiming.Update, AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                    return;
            }
        }
    }
}