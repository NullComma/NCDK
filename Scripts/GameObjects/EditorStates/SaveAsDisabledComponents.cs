using System.Collections.Generic;
using UnityEngine;

namespace EnigmaCore
{
    /// <summary>
    /// Forces a list of specific components to be disabled when saving or entering play mode.
    /// </summary>
    public class SaveAsDisabledComponents : EditorStateEnforcer
    {
        public List<Behaviour> targetComponents = new();

#if UNITY_EDITOR
        protected override void EnforceState()
        {
            bool changed = false;
            foreach (var component in targetComponents)
            {
                if (component != null && component.enabled)
                {
                    component.enabled = false;
                    changed = true;
                }
            }

            if (changed)
            {
                Debug.Log($"[EnigmaCore] Components on '{this.name}' set to DISABLED by CSaveAsDisabledComponents.");
            }
        }
#endif
    }
}