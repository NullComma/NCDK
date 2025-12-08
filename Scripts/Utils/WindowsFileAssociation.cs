#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace EnigmaCore
{
    public static class WindowsFileAssociation
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterFileAssociation()
        {
            // O bloco try-catch aqui é a última linha de defesa.
            // Se algo falhar, ele engole o erro e o jogo abre normalmente.
            try
            {
                // 1. Maneira segura de achar o EXE sem usar System.Diagnostics
                // Application.dataPath retorna ".../NomeDoJogo_Data"
                string dataPath = Application.dataPath;
                if (!dataPath.EndsWith("_Data")) return; // Estrutura de pasta estranha, aborta silenciosamente.

                // Converte ".../NomeDoJogo_Data" para ".../NomeDoJogo.exe"
                string exePath = dataPath.Substring(0, dataPath.Length - 5) + ".exe";

                if (!File.Exists(exePath)) return; // Se não achou o exe, aborta.

                string extension = EnigmaPaths.SaveExtension;
                
                // Remove espaços do ID para evitar problemas no registro
                string progId = $"{Application.companyName}.{Application.productName}.SaveFile".Replace(" ", "");

                // 2. Registra Extensão -> ProgID
                SetRegistryKey($@"HKCU\Software\Classes\{extension}", "", progId);

                // 3. Registra ProgID -> Comando de Abrir
                string openCommand = $"\"{exePath}\" \"%1\"";
                SetRegistryKey($@"HKCU\Software\Classes\{progId}\shell\open\command", "", openCommand);

                // 4. (Opcional) Ícone Padrão
                string iconValue = $"\"{exePath}\",0";
                SetRegistryKey($@"HKCU\Software\Classes\{progId}\DefaultIcon", "", iconValue);

                // 5. Atualiza ícones do Explorer
                SHChangeNotify(0x08000000, 0, IntPtr.Zero, IntPtr.Zero);
            }
            catch
            {
                // Silêncio absoluto em caso de erro.
                // Não loga nada para não assustar, apenas permite que o jogo continue.
            }
        }

        private static void SetRegistryKey(string keyPath, string valueName, string valueData)
        {
            try
            {
                string valueSwitch = string.IsNullOrEmpty(valueName) ? "/ve" : $"/v \"{valueName}\"";
                string args = $"add \"{keyPath}\" {valueSwitch} /d \"{valueData}\" /f";

                var processInfo = new ProcessStartInfo("reg.exe", args)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var process = Process.Start(processInfo))
                {
                    process.WaitForExit(500); // Espera no máximo meio segundo
                }
            }
            catch
            {
                // Ignora falhas no reg.exe
            }
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(int wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}
#endif