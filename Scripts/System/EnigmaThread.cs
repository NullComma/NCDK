using System.Threading;
using UnityEngine;

namespace NCDK {
    public static class EnigmaThread {

        public static Thread MainThread { get; private set; } 
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void InitializeBeforeSplashScreen() {
            MainThread = Thread.CurrentThread;
        }
        
    }
}