using Cysharp.Threading.Tasks;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace K4.Threading
{
	public partial class K4UnityThreadDispatcher
	{
		private static readonly ConcurrentQueue<Action> globalPendingActions = new ConcurrentQueue<Action>();

		public static UniTask<R> Execute<R>(Func<R> func)
		{
			UniTaskCompletionSource<R> tcs = new UniTaskCompletionSource<R>();
			void InternalAction()
			{
				try
				{
					R returnValue = func();
					tcs.TrySetResult(returnValue);
				}
				catch (Exception e)
				{
					tcs.TrySetException(e);
				}
			}

			globalPendingActions.Enqueue(InternalAction);
			return tcs.Task;
		}

		public static UniTask Execute(Action action)
		{
			return Execute(
				() => {
					action();
					return true;
				}
			);
		}
	}
}
