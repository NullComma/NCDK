namespace EnigmaCore {
	public class DontDestroyOnLoadTrigger : AutoTriggerCompBase {
		protected override void TriggerEvent() {
			gameObject.DontDestroyOnLoad();
		}
	}
}