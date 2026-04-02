using UnityEditor;
using UnityEngine;

namespace EnigmaCore.Editor
{
    /// <summary>
    /// Editor window and automated routine to manage Android Keystore credentials per package name.
    /// Hooks into InitializeOnLoad to automatically apply the credentials when the Unity Editor opens.
    /// </summary>
    [InitializeOnLoad]
    public class AndroidKeystoreManager : EditorWindow
    {
        [System.NonSerialized]
        string keystorePass = "";

        [System.NonSerialized]
        string keyaliasName = "";

        [System.NonSerialized]
        string keyaliasPass = "";

        static AndroidKeystoreManager()
        {
            EditorApplication.delayCall += ApplySavedCredentials;
        }

        /// <summary>
        /// Retrieves the saved credentials for the current Android package name and applies them.
        /// </summary>
        static void ApplySavedCredentials()
        {
            PlayerSettings.Android.keystorePass = EditorPrefs.GetString(GetPackageSpecificKey("KeystorePass"), "");
            PlayerSettings.Android.keyaliasName = EditorPrefs.GetString(GetPackageSpecificKey("KeyaliasName"), "");
            PlayerSettings.Android.keyaliasPass = EditorPrefs.GetString(GetPackageSpecificKey("KeyaliasPass"), "");
        }

        /// <summary>
        /// Generates a unique EditorPrefs key bound to the current Android application identifier.
        /// </summary>
        static string GetPackageSpecificKey(string baseKey)
        {
            string packageName = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            return $"{packageName}_{baseKey}";
        }

        [MenuItem("Tools/Android Keystore Manager")]
        public static void ShowWindow()
        {
            GetWindow<AndroidKeystoreManager>("Keystore Manager");
        }

        void OnEnable()
        {
            keystorePass = EditorPrefs.GetString(GetPackageSpecificKey("KeystorePass"), "");
            keyaliasName = EditorPrefs.GetString(GetPackageSpecificKey("KeyaliasName"), "");
            keyaliasPass = EditorPrefs.GetString(GetPackageSpecificKey("KeyaliasPass"), "");
        }

        void OnGUI()
        {
            GUILayout.Label("Package Specific Android Credentials", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Passwords are saved in plain text in the OS registry.", MessageType.Info);

            EditorGUILayout.Space();

            keystorePass = EditorGUILayout.PasswordField("Keystore Password", keystorePass);
            keyaliasName = EditorGUILayout.TextField("Keyalias Name", keyaliasName);
            keyaliasPass = EditorGUILayout.PasswordField("Keyalias Password", keyaliasPass);

            EditorGUILayout.Space();

            if (GUILayout.Button("Save and Apply"))
            {
                EditorPrefs.SetString(GetPackageSpecificKey("KeystorePass"), keystorePass);
                EditorPrefs.SetString(GetPackageSpecificKey("KeyaliasName"), keyaliasName);
                EditorPrefs.SetString(GetPackageSpecificKey("KeyaliasPass"), keyaliasPass);

                ApplySavedCredentials();
                Debug.Log($"Android Keystore credentials saved locally for package: {PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android)}");
            }
        }
    }
}