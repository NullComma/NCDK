using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EnigmaCore {
	public static class MonoBehaviourExtensions {
		
		public static Coroutine CStartCoroutine(this MonoBehaviour monoBehaviour, IEnumerator coroutine) {
			if(EApplication.IsQuitting) {
				Debug.LogError("Tried to start a coroutine while application is quitting. Coroutine will not be started.", monoBehaviour);
				return null;
			}
			if(monoBehaviour == null) {
				Debug.LogError("Tried to start a coroutine on a null MonoBehaviour. Coroutine will not be started.");
				return null;
			}
			if(coroutine == null) {
				Debug.LogError("Tried to start a null coroutine.");
				return null;
			}
			if(!monoBehaviour.isActiveAndEnabled) {
				Debug.LogError("Cannot start coroutine in a disabled MonoBehaviour.", monoBehaviour);
				return null;
			}
			return monoBehaviour.StartCoroutine(coroutine);
		}
		
		public static void CStopCoroutine(this MonoBehaviour monoBehaviour, Coroutine coroutine) {
			if(coroutine == null) return;
			if(monoBehaviour == null) return;
			monoBehaviour.StopCoroutine(coroutine);
		}
        
        public static void CResolveComponentFromChildrenIfNull<T>(this MonoBehaviour m, ref T c) where T : Component {
            if (c != null) return;
            c = m.GetComponentInChildren<T>();
        }
        
        public static void CResolveComponentFromChildrenOrParentIfNull<T>(this MonoBehaviour m, ref T c) where T : Component {
            if (c != null) return;
            c = m.CGetComponentInChildrenOrInParent<T>();
        }

        public static void CSetNameIfOnlyComponent(this MonoBehaviour m, string name) {
            if (m == null) return;
            var comps = m.GetComponents<Component>();
            if (comps == null) return;
            if (comps.Length > 2) return;
            m.name = name;
            #if UNITY_EDITOR
            EditorUtility.SetDirty(m);
            #endif
        }

		public static void CheckForDuplicatedIds(this ISerializedObject m)
		{
			var others = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
				.OfType<ISerializedObject>()
				.ToList();
			others.Remove(m);
			if (others.Count <= 0) return;
			foreach (var other in others)
			{
				if (other.ID == m.ID)
				{
					Debug.LogError($"Found duplicated ID {m.ID} in object '{other}' and '{m}'. Resetting ID of '{m}'.", (MonoBehaviour)m);
					#if UNITY_EDITOR
					//overwrite by reflection 'm.ID = SerializableGuid.NewGuid();':
					var field = m.GetType().GetField("id", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
					if (field != null) field.SetValue(m, SerializableGuid.NewGuid());
					Selection.activeObject = (MonoBehaviour)m;
					#endif
					break;
				}
			}
		}
		
		public static MonoBehaviour DontDestroyOnLoad(this MonoBehaviour mb) {
			Debug.Log($"Setting '{mb.name}' to DontDestroyOnLoad");
			Object.DontDestroyOnLoad(mb);
			return mb;
		}

    }
}