using System;
using UnityEditor;

namespace SCKRM.Editor
{
    public static class BuildHandler
    {
        /// <summary>
        /// Returning false cancels the build
        /// </summary>
        public static event Func<bool> onBuildStarted;
        public static event Action onBuildSuccess;
        public static event Action onBuildFailure;

        [InitializeOnLoadMethod]
        static void Init() => BuildPlayerWindow.RegisterBuildPlayerHandler(HandleBuildPlayer);

        static void HandleBuildPlayer(BuildPlayerOptions buildOptions)
        {
            try
            {
                Delegate[] delegates = onBuildStarted?.GetInvocationList();
                if (delegates != null)
                {
                    for (int i = 0; i < delegates.Length; i++)
                    {
                        Func<bool> func = (Func<bool>)delegates[i];
                        if (!func.Invoke())
                        {
                            onBuildFailure?.Invoke();
                            return;
                        }
                    }
                }

                BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(buildOptions);
                onBuildSuccess?.Invoke();
            }
            catch (Exception e)
            {
                Exception log = e.InnerException ?? e;
                Debug.LogException(log);

                onBuildFailure?.Invoke();
            }
        }
    }
}
