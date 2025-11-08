using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace EnigmaCore {
    public class RaiseBuildVersion : IPostprocessBuildWithReport {

        public int callbackOrder => 9999;

        public void OnPostprocessBuild(BuildReport report) {
            if (report.summary.result != BuildResult.Succeeded && report.summary.result != BuildResult.Unknown) return;
            Debug.Log($"Build version {Application.version} succeeded.");
            CEditorPlayerSettings.RaiseBuildVersion();
        }

    }
}