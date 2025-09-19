using System;
using System.Threading.Tasks;
using UnityEngine;

namespace EnigmaCore {
	public static class TaskExtensions {

		public static async void Await(this Task task, bool continueOnCapturedContext = true)
		{
			try {
				await task.ConfigureAwait(continueOnCapturedContext);
			}
			catch (Exception e) {
				Debug.LogException(e);
			}
		}

		public static bool IsRunning(this Task task) {
			return task != null && !task.IsCompleted;
		}
	}
}
