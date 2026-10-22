#if UNITY_EDITOR
using System.IO;
using UnityEditor;

namespace NCDK
{
    /// <summary>
    /// Editor-only bridge between the Save File Editor and Play Mode: the file chosen in the window
    /// is the one the game loads when Play starts. With no selection the normal behaviour stays —
    /// load the most recent save, or create a new one.
    /// </summary>
    /// <remarks>
    /// Stored in <see cref="EditorPrefs"/> so it survives domain reloads and Play sessions; deleted
    /// as soon as the "None" entry is selected.
    /// </remarks>
    public static class SaveEditorSelection
    {
        const string PrefKey = "NCDK.SaveEditor.PlaySavePath";

        /// <summary>Full path of the save to load on Play, or an empty string for "None".</summary>
        public static string SelectedPath
        {
            get => EditorPrefs.GetString(PrefKey, string.Empty);
            set
            {
                if (string.IsNullOrEmpty(value)) EditorPrefs.DeleteKey(PrefKey);
                else EditorPrefs.SetString(PrefKey, value);
            }
        }

        public static bool HasSelection => !string.IsNullOrEmpty(SelectedPath);

        /// <summary>True when a save is selected and still exists on disk.</summary>
        public static bool TryGetPath(out string path)
        {
            path = SelectedPath;
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }

        /// <summary>Back to the default behaviour: no override.</summary>
        public static void Clear() => EditorPrefs.DeleteKey(PrefKey);
    }
}
#endif
