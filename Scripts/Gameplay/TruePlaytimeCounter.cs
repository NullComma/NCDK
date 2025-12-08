using System;
using EnigmaCore;
using EnigmaCore.DependecyInjection;
using UnityEngine;

namespace Game
{
    public class TruePlaytimeCounter : MonoBehaviour
    {
        public TimeSpan Duration => TimeSpan.FromSeconds(_accumulatedTime);
        [Inject] [NonSerialized] CBlockingEventsManager _blockingEventsManager;
        
        [NonSerialized] double _accumulatedTime;

        void Awake()
        {
            this.Inject();
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.DontSave; 
        }

        void Update()
        {
            if (Time.deltaTime <= 0.0001f) return;
            
            if (_blockingEventsManager != null && _blockingEventsManager.InMenuOrPlayingCutscene) return;

            _accumulatedTime += Time.deltaTime;
        }

        public void ResetCounter()
        {
            _accumulatedTime = 0;
        }
    }
}