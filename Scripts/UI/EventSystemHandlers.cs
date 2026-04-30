using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NCDK.UI
{
    public class EventSystemHandlers : MonoBehaviour, ICancelHandler
    {
        public event Action<BaseEventData> CancelEvent = delegate { }; 
        
        public void OnCancel(BaseEventData eventData) => CancelEvent.Invoke(eventData);
    }
}