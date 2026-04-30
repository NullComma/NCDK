using UnityEngine;

namespace NCDK
{
    /// <summary>
    /// Forces the GameObject to be active when saving or entering play mode.
    /// </summary>
    public class SaveAsActiveGameObject : EditorStateEnforcer
    {
#if UNITY_EDITOR
        protected override void EnforceState()
        {
            if (this.gameObject.activeSelf) return;

            this.gameObject.SetActive(true);
            Debug.Log($"[NCDK] '{this.name}' set to ACTIVE by CSaveAsActiveGameObject.");
        }
#endif
    }
}