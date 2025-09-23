using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

namespace EnigmaCore.Editor 
{
    internal static class RenameSelectedComponent 
    {
        [MenuItem("CONTEXT/Component/Rename GameObject with this component name")]
        private static void RenameGameObjectWithThisComponentName(MenuCommand data) 
        {
            var component = data.context as Component;
            if (component == null) return;

            string formattedName = FormatComponentName(component.GetType().Name);

            Undo.RecordObject(component.gameObject, $"Rename to '{formattedName}'");
            component.gameObject.name = formattedName;
        }

        /// <summary>
        /// Formats a class name from PascalCase/CamelCase into a user-friendly string.
        /// e.g., "CExampleComponent" becomes "Example Component".
        /// e.g., "MyURLHandler" becomes "My URL Handler".
        /// </summary>
        /// <param name="name">The component's class name to format.</param>
        /// <returns>A formatted, human-readable name.</returns>
        private static string FormatComponentName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;

            // Inserts a space before an uppercase letter that is either preceded by a lowercase letter
            // or preceded by another uppercase letter and followed by a lowercase letter.
            // Handles both "MyScript" -> "My Script" and "URLHandler" -> "URL Handler".
            string spacedName = Regex.Replace(name, "(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])", " ");

            // Split the name into words to check for a single-character prefix.
            string[] words = spacedName.Split(' ');

            // If the first word is a single uppercase character (like 'C' or 'T'), remove it.
            if (words.Length > 1 && words[0].Length == 1 && char.IsUpper(words[0][0]))
            {
                // Join the remaining words back together.
                return string.Join(" ", words.Skip(1));
            }

            return spacedName;
        }
    }
}