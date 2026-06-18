using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace NCDK
{
    /// <summary>
    /// A collection of UnityEvents for handling state changes.
    /// </summary>
    [Serializable]
    public class StateUnityEvents
    {
        [SerializeField] UnityEvent On = new();
        [SerializeField] UnityEvent Off = new();
        [SerializeField] CUnityEventBool StateInverted = new();
        [SerializeField, FormerlySerializedAs("State")] CUnityEventBool _state = new();

        /// <summary>Exposes the State event for external subscribers.</summary>
        public CUnityEventBool State => _state;

        public void Trigger(bool value)
        {
            _state.Invoke(value);
            StateInverted.Invoke(!value);
            if (value)
                On.Invoke();
            else
                Off.Invoke();
        }
    }
}