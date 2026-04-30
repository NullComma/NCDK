using System;
using Unity.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NCDK {
    public static class ComponentExtensions {

        public static T GetComponentInChildrenOrInParent<T>(this Component comp, bool includeInactive = false) {
            return comp.gameObject.CGetComponentInChildrenOrInParent<T>(includeInactive);
        }
        
        public static T GetComponentInParentOrInChildren<T>(this Component comp, bool includeInactive = false) {
            return comp.gameObject.CGetComponentInParentOrInChildren<T>(includeInactive);

        }
        
        public static T GetComponentInChildrenRecursiveUntilRoot<T>(this Component comp, bool includeInactive = false) {
            if (comp == null || comp.gameObject == null) return default;
            T target = default;
            foreach (var ancestor in comp.gameObject.AncestorsAndSelf()) {
                target = ancestor.GetComponentInChildren<T>(includeInactive);
                if (target != null) {
                    return target;
                }
            }
            return target;
        }
        
        public static void DestroyGameObject(this Component value, float time = 0f) {
            if (value == null || value.gameObject == null) return;
            if (Application.isPlaying) {
                Object.Destroy(value.gameObject, time > 0f ? time : 0f);
            }
            else {
                Object.DestroyImmediate(value.gameObject);
            }
        }

        public static T GetOrAddComponent<T>(this Component value) where T : Component {
            if (value == null) {
                Debug.LogError($"Cant add component to a null component!");
                return default;
            }

            var comp = value.GetComponent<T>();
            return comp ? comp : value.gameObject.AddComponent<T>();
        }
        
        public static bool AssertIfNull<T>(this T c, string message = null, Object source = null) where T : Component {
            bool isNull = (c == null);
            if (isNull) {
                Debug.LogError($"<b>Assert</b>: Component is null ({message})", source);
            }
            return isNull;
        }
        
        public static bool ThrowIfNull<T>(this T c, string message = null, Object source = null) where T : Component {
            bool isNull = (c == null);
            if (isNull)
            {
                var exception = new NullReferenceException($"Component is null ({message})");
                Debug.LogException(exception, source);
                throw exception;
            }
            return isNull;
        }

        public static void DestroyGameObject<T>(this T value, bool shouldLog = false, float time = 0f) where T : Component {
            if (value == null) return;
            value.gameObject.CDestroy(shouldLog, time);
        }

    }
}