using UnityEngine;

namespace NullCore.Refs
{
    public class ValidatedMonoBehaviour : MonoBehaviour
    {
        protected virtual void Reset()
        {
#if UNITY_EDITOR
            this.ValidateRefs();
#endif
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            this.ValidateRefs();
        }
#else
        protected virtual void OnValidate() { }
#endif
    }
}
