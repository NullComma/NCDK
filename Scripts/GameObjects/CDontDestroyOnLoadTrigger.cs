namespace EnigmaCore {
	public class CDontDestroyOnLoadTrigger : CAutoTriggerCompBase {
		protected override void TriggerEvent() {
			gameObject.CDontDestroyOnLoad();
		}
	}
}