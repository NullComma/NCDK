using System;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EnigmaCore
{
    
    /// <summary>
    /// The InputSystemUIInputModule when disabled disposes the reference to the Input Action.
    /// This component restores the reference to the Input Action.
    /// </summary>
    #if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(InputSystemUIInputModule))]
    [DefaultExecutionOrder(1)]
    #endif
    public class RestoreInputActionOnEnable : MonoBehaviour
    {
        #if ENABLE_INPUT_SYSTEM

        InputSystemUIInputModule _uiModule;
        InputActionAsset _originalInputActionAsset;
        
        #if UNITY_EDITOR
        [MenuItem("CONTEXT/RestoreInputActionOnEnable/Add RestoreInputActionOnEnable")]
        static void AddRestoreInputActionOnEnable(MenuCommand data) {
            if (data?.context is not RestoreInputActionOnEnable comp) return;
            comp.CGetOrAddComponent<RestoreInputActionOnEnable>();
            Debug.Log("Added RestoreInputActionOnEnable");
            EditorUtility.SetDirty(comp);
        }
        #endif
        
        void Awake()
        {
            TryGetComponent(out _uiModule);
            _originalInputActionAsset = _uiModule.actionsAsset;
        }
        
        void OnEnable()
        {
            _uiModule.actionsAsset = _originalInputActionAsset;
        }

        #if UNITY_EDITOR
        void OnValidate()
        {
            if (TryGetComponent(out _uiModule))
            {
                EditorUtility.SetDirty(this);
            }
            _originalInputActionAsset = _uiModule.actionsAsset ;
        }

        void Reset()
        {
            OnValidate();
        }
        #endif

        #endif
    }
}