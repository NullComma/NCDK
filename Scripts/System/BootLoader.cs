using System;
using System.IO;
using UnityEngine;

namespace EnigmaCore
{
    public static class BootLoader
    {
        /// <summary>
        /// Returns the file path if the game was launched via double-click on a save file.
        /// Returns null if launched normally.
        /// </summary>
        public static string GetFileFromLaunchArgs()
        {
#if UNITY_STANDALONE_WIN
            string[] args = Environment.GetCommandLineArgs();

            // args[0] is the executable path.
            // args[1] is the file path (if opened via file association).
            if (args.Length > 1)
            {
                string possibleFilePath = args[1];
                
                // Validate if it is indeed our file
                if (File.Exists(possibleFilePath) && possibleFilePath.EndsWith(EnigmaPaths.SaveExtension))
                {
                    Debug.Log($"[BootLoader] Launched via save file: {possibleFilePath}");
                    return possibleFilePath;
                }
            }
#endif
            
            return null;
        }
    }
}