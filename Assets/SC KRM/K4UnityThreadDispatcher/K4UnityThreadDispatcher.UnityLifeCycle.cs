using System.Diagnostics;
using System.Management.Instrumentation;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace K4.Threading
{
	using SelfType = K4UnityThreadDispatcher;

	public partial class K4UnityThreadDispatcher : MonoBehaviour
	{
		public static SelfType Instance
		{
			get;
			private set;
		}

		public const int AllotedTimeEachWindow = 3;

		private static readonly Stopwatch windowTimeStopwatch = new Stopwatch();

		private void Update()
		{
			windowTimeStopwatch.Restart();

			while (globalPendingActions.TryDequeue(out System.Action action) && windowTimeStopwatch.Elapsed.TotalMilliseconds < AllotedTimeEachWindow)
				action();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		public static void CreateDispatcher()
        {
            if(Instance == null)
			{
#if UNITY_2023_1_OR_NEWER
                SelfType dispatcher = FindFirstObjectByType<SelfType>() ?? new GameObject("Unity Thread Dispatcher").AddComponent<SelfType>();
#else
                SelfType dispatcher = FindObjectOfType<SelfType>() ?? new GameObject("Unity Thread Dispatcher").AddComponent<SelfType>();
#endif
				DontDestroyOnLoad(dispatcher);
				Instance = dispatcher;
			}
		}
	}
}

