using System.Diagnostics;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace NCDK {
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
            // Derive folder and product name from the build output itself. PlayerSettings.productName
            // can already be back to the project default when this postprocess runs (a pre-build
            // preprocessor reimports ProjectSettings), and then the replace below stripped nothing:
            // the shortcut was written inside "<exe>.exe\Logs and Save Directory.lnk" and the target
            // pointed at the wrong LocalLow folder.
            var outputPath = Path.GetDirectoryName(buildPath);
            var builtProductName = Path.GetFileNameWithoutExtension(buildPath);
            CreateShortcutToApplicationLogsDirectoryOnWindows(outputPath, builtProductName);
            #endif
        }

        static void CreateShortcutToApplicationLogsDirectoryOnWindows(string outputPath, string productName)
        {
            string targetPath = Path.Combine("%USERPROFILE%\\AppData\\LocalLow", Application.companyName, productName);
            string shortcutPath = Path.Combine(outputPath, "Logs and Save Directory.lnk");

            if (!Directory.Exists(outputPath))
            {
                Debug.LogError($"[CreateShortcut] Build output folder not found: '{outputPath}'");
                return;
            }

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