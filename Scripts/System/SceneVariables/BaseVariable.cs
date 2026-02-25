using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EnigmaCore.SceneVariables
{
    public class BaseVariable : MonoBehaviour
    {
        [SerializeField,ReadOnly] protected SceneVariableManager Manager;
        
        void OnValidate()
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EnsureManagerExists();
            }
            #endif
        }

        #if UNITY_EDITOR
        void EnsureManagerExists()
        {
            if (Manager != null) return;
            var searched = FindAnyObjectByType<SceneVariableManager>(FindObjectsInactive.Include);
            if (searched != null) Manager = searched;
            else Manager = new GameObject("SceneVariableManager").AddComponent<SceneVariableManager>();
            Manager.transform.SetAsFirstSibling();
            EditorUtility.SetDirty(this);
        }
        #endif
    }
}