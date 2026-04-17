using System;
using System.Threading.Tasks;
using UnityEngine;

namespace NullCore {
	public static class TaskExtensions {

		public static async void Await(this Task task, bool continueOnCapturedContext = true)
		{
			try {
				await task.ConfigureAwait(continueOnCapturedContext);
			}
			catch (Exception e) {
				Debug.LogError($"[TaskExtensions] Exception in Await: {e.Message}\n{e.StackTrace}");
			}
		}

		public static bool IsRunning(this Task task) {
			return task != null && !task.IsCompleted;
		}
	}
}
