using System;
using System.Collections;
using UnityEngine;
using Screen = UnityEngine.Device.Screen;

namespace EnigmaCore {
    public static class EnigmaQualitySettings {

        public static bool GetVsync() {
            return QualitySettings.vSyncCount == 1;
        }

        public static void SetVsync(bool value) {
            QualitySettings.vSyncCount = value ? 1 : 0;
        }

    }
}