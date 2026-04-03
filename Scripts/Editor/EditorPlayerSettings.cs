using System;
using UnityEditor;

namespace NullCore {
    public static class EditorPlayerSettings {

        public static void RaiseBuildVersion() {
            if (!Version.TryParse(PlayerSettings.bundleVersion, out var version)) return;
            PlayerSettings.bundleVersion = new Version(version.Major, version.Minor, version.Build + 1).ToString();
            PlayerSettings.Android.bundleVersionCode++;
            if (int.TryParse(PlayerSettings.iOS.buildNumber, out var iosBuildNumber)) {
                PlayerSettings.iOS.buildNumber = (iosBuildNumber + 1).ToString();
            } else {
                PlayerSettings.iOS.buildNumber = "1";
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
    }
}