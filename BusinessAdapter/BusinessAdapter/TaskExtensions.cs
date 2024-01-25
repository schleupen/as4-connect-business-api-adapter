// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	internal static class TaskExtensions
	{
		/// <summary>
		/// Allows the configuration of a cancellation for a task.
		/// </summary>
		/// <typeparam name="T">The return type of the task.</typeparam>
		/// <param name="task">The task.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The configured task.</returns>
		/// <exception cref="OperationCanceledException"></exception>
		public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
		{
			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
			using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s!).TrySetResult(true), taskCompletionSource))
			{
				if (task != await Task.WhenAny(task, taskCompletionSource.Task))
				{
					throw new OperationCanceledException(cancellationToken);
				}
			}

			return await task;
		}
	}
}
