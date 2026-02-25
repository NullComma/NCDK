using System;
using UnityEngine;
using UnityEngine.Events;

namespace EnigmaCore
{
    /// <summary>
    /// A collection of UnityEvents for handling state changes.
    /// </summary>
    [Serializable]
    public class StateUnityEvents
    {
        [SerializeField] UnityEvent On = new();
        [SerializeField] UnityEvent Off = new();
        [SerializeField] CUnityEventBool State = new();
        [SerializeField] CUnityEventBool StateInverted = new();

        public void Trigger(bool value)
        {
            State.Invoke(value);
            StateInverted.Invoke(!value);
            if (value)
                On.Invoke();
            else
                Off.Invoke();
        }
    }
}