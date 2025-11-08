using System.Diagnostics;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace EnigmaCore {
    public class CreateShortcut : IPostprocessBuildWithReport {

        public int callbackOrder => 1;

        public void OnPostprocessBuild(BuildReport report) {
            if (report.summary.result != BuildResult.Succeeded && report.summary.result != BuildResult.Unknown) return;
            Debug.Log("Running OnPostprocessBuild() for shortcut creation");
            string buildPath = report.summary.outputPath;

            #if UNITY_WEBGL
            string netlifyFolder = Path.Combine(buildPath, ".netlify").Replace("\\", "/");
            if (Directory.Exists(netlifyFolder))
            {
                Debug.Log($".netlify folder found in: {netlifyFolder}. Executing cmd 'netlify build --prod'...");
                NetlifyBuildProd(netlifyFolder, "netlify build --prod", true);
            }
            #elif UNITY_EDITOR_WIN
            var outputPath = buildPath.Replace($"/{Application.productName}.exe", string.Empty);
            CreateShortcutToApplicationLogsDirectoryOnWindows(outputPath);
            #endif
        }

        static void CreateShortcutToApplicationLogsDirectoryOnWindows(string outputPath)
        {
            string targetPath = Path.Combine("%USERPROFILE%\\AppData\\LocalLow", Application.companyName, Application.productName);
            string shortcutPath = Path.Combine(outputPath, "Logs and Save Directory.lnk");

            string cmd = $"/c powershell -Command \"$WshShell = New-Object -ComObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('{shortcutPath}'); $Shortcut.TargetPath = '{targetPath}'; $Shortcut.Save()\"";

            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = cmd,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            UnityEngine.Debug.Log($"Shortcut creation output: {output}");
        }

#if UNITY_WEBGL
        static void NetlifyBuildProd(string cdToPath, string cmd, bool createWindow = false)
        {
            try
            {
                using var process = new Process();
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C cd \"{cdToPath}\" && {cmd}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = !createWindow
                };
                process.OutputDataReceived += (sender, e) => Debug.Log(e.Data);
                process.ErrorDataReceived += (sender, e) => Debug.LogError(e.Data);
                process.Start();
                process.WaitForExit();
                Debug.Log($"Command '{cmd}' completed.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error executing '{cmd}': {ex.Message}");
            }
        }
#endif

    }
}