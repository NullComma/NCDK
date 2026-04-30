using UnityEngine;
using UnityEngine.SceneManagement;

namespace NCDK {
	public class SceneReloader : MonoBehaviour {
		
		public void ReloadCurrentScene() {
			var activeScene = SceneManager.GetActiveScene();
			SceneManager.LoadScene(activeScene.buildIndex);
		}
		
		public void ReloadCurrentSceneAsync() {
			var activeScene = SceneManager.GetActiveScene();
			SceneManager.LoadSceneAsync(activeScene.buildIndex);
		}
	}
}
