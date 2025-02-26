using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EnigmaCore.EnigmaCore.Scripts.UI
{
    public class EventSystemHandlers : MonoBehaviour, ICancelHandler
    {
        public event Action<BaseEventData> CancelEvent = delegate { }; 
        
        public void OnCancel(BaseEventData eventData) => CancelEvent.Invoke(eventData);
    }
}