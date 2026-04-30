using UnityEngine;

namespace NCDK
{
    /// <summary>
    /// Forces the GameObject to be inactive when saving or entering play mode.
    /// </summary>
    public class SaveAsInactiveGameObject : EditorStateEnforcer
    {
#if UNITY_EDITOR
        protected override void EnforceState()
        {
            if (!this.gameObject.activeSelf) return;
            
            this.gameObject.SetActive(false);
            Debug.Log($"[NCDK] '{this.name}' set to INACTIVE by CSaveAsInactiveGameObject.");
        }
#endif
    }
}