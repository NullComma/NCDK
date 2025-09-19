using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace EnigmaCore
{
    /// <summary>
    /// Automatically adds or removes the "FMOD" scripting define symbol
    /// based on whether the FMOD Unity integration classes are present in the project.
    /// </summary>
    [InitializeOnLoad]
    public class FmodDefineManager
    {
        private const string FMOD_DEFINE = "FMOD";

        static FmodDefineManager()
        {
            // Get the current build target group
            var buildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            // Get the current scripting defines
            string definesString = PlayerSettings.GetScriptingDefineSymbols(buildTarget);
            List<string> allDefines = definesString.Split(';').ToList();

            bool isFmodPresent = IsFmodAssemblyPresent();
            bool isDefineSet = allDefines.Contains(FMOD_DEFINE);

            // Add the define if FMOD is present but the define is not set
            if(isFmodPresent && !isDefineSet)
            {
                allDefines.Add(FMOD_DEFINE);
                PlayerSettings.SetScriptingDefineSymbols(buildTarget,
                    string.Join(";", allDefines.ToArray()));
                UnityEngine.Debug.Log("FMOD detected. 'FMOD' scripting define symbol has been added.");
            }
            // Remove the define if FMOD is not present but the define is set
            else if(!isFmodPresent && isDefineSet)
            {
                allDefines.Remove(FMOD_DEFINE);
                PlayerSettings.SetScriptingDefineSymbols(buildTarget,
                    string.Join(";", allDefines.ToArray()));
                UnityEngine.Debug.Log("FMOD not detected. 'FMOD' scripting define symbol has been removed.");
            }
        }

        /// <summary>
        /// Checks if any loaded assembly contains types from the FMODUnity namespace.
        /// This is a reliable way to check for the FMOD package's presence.
        /// </summary>
        private static bool IsFmodAssemblyPresent()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // FMODUnity is the core namespace for the integration
                if(assembly.FullName.StartsWith("FMODUnity,"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}