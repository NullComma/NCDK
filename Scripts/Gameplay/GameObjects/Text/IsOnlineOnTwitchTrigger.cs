using UnityEngine;
using UnityEngine.Networking;

namespace NullCore.Text {
	/// <summary>
	/// Dunno if this works in another languages like Japanese.
	/// </summary>
	public class IsOnlineOnTwitchTrigger : AutoTriggerCompBase {

		[SerializeField] private string _userName = "chrisdbhr";
		[SerializeField] private CUnityEventBool _isOnline;
		
		
		
		
		protected override void TriggerEvent() {
			var get = UnityWebRequest.Get($"https://twitch.tv/{this._userName}");
			get.SendWebRequest().completed += asyncOp => {
                if (this == null) return;
				this._isOnline?.Invoke(get.downloadHandler.text.Contains("isLiveBroadcast"));
				get.Dispose();
			};
		}
	}
}
