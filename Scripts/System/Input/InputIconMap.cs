using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NullCore.Input
{
    [Serializable]
    public struct InputIconBinding
    {
        [Tooltip("The path of the control. Picked via the InputControlPicker attribute.")]
        [InputControlPicker]
        public string controlPath;
        
        [Header("Icons")]
        [Tooltip("Sprites for Keyboard and Mouse.")]
        public Sprite[] keyboardMouseIcons;
        
        [Tooltip("Sprites for Xbox-like layout.")]
        public Sprite[] xboxIcons;

        [Tooltip("Sprites for PlayStation-like layout.")]
        public Sprite[] playStationIcons;

        [Tooltip("Sprites for Nintendo-like layout.")]
        public Sprite[] nintendoIcons;
        
        [Header("Animation Settings")]
        public float animationFPS;
        
        public float DefaultFPS => animationFPS > 0 ? animationFPS : 10f;
    }

    [CreateAssetMenu(fileName = "InputIconMap", menuName = "EnigmaCore/Input/Icon Map")]
    public class InputIconMap : ScriptableObject
    {
        public List<InputIconBinding> bindings = new List<InputIconBinding>();

        // Cache for runtime performance
        private Dictionary<string, InputIconBinding> _runtimeMap;

        private void OnEnable()
        {
            BuildRuntimeMap();
        }

        public void BuildRuntimeMap()
        {
            _runtimeMap = new Dictionary<string, InputIconBinding>();
            foreach (var binding in bindings)
            {
                if (!string.IsNullOrEmpty(binding.controlPath) && !_runtimeMap.ContainsKey(binding.controlPath))
                {
                    _runtimeMap.Add(binding.controlPath, binding);
                }
            }
        }

        public bool TryGetBinding(string controlPath, out InputIconBinding binding)
        {
            if (_runtimeMap == null || _runtimeMap.Count == 0 || _runtimeMap.Count != bindings.Count)
            {
                BuildRuntimeMap();
            }

            return _runtimeMap.TryGetValue(controlPath, out binding);
        }
    }
}
