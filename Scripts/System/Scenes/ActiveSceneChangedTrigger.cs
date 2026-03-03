using UnityEngine.SceneManagement;

namespace EnigmaCore {
	public class ActiveSceneChangedTrigger : UnityEventTrigger{

		public void OnEnable() {
			SceneManager.activeSceneChanged += this.ActiveSceneChanged;
		}

		public void OnDisable() {
			SceneManager.activeSceneChanged -= this.ActiveSceneChanged;
		}

		private void ActiveSceneChanged(Scene oldScene, Scene newScene) {
			this.TriggerEvent();
		}

	}
}