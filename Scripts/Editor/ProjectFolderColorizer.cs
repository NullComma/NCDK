using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace EnigmaticComma.Editor
{
    /// <summary>
    /// Renders a subtle gradient background for specific project folders.
    /// </summary>
    [InitializeOnLoad]
    public static class ProjectFolderGradient
    {
        // Settings: Reduced Alpha for subtlety (0.15f - 0.25f is ideal for gradients)
        private static readonly Dictionary<string, Color> FolderColors = new Dictionary<string, Color>
        {
            { "Scenes",     new Color(1.0f, 0.3f, 0.3f, 0.2f) }, 
            { "Scripts",    new Color(0.3f, 1.0f, 0.3f, 0.2f) }, 
            { "Editor",     new Color(1.0f, 0.9f, 0.2f, 0.2f) }, 
            { "Resources",  new Color(0.2f, 0.8f, 1.0f, 0.2f) }, 
            { "Plugins",    new Color(0.7f, 0.4f, 1.0f, 0.2f) }, 
            { "Prefabs",    new Color(1.0f, 0.6f, 0.2f, 0.2f) }, 
            { "Audio",      new Color(1.0f, 0.4f, 0.8f, 0.2f) }, 
            { "Materials",  new Color(0.8f, 0.8f, 0.8f, 0.2f) }, 
            { "Textures",   new Color(0.6f, 0.2f, 0.8f, 0.2f) }, 
            { "StreamingAssets", new Color(0.2f, 1.0f, 0.8f, 0.2f) } 
        };

        private static Texture2D _gradientTexture;

        static ProjectFolderGradient()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowGUI;
        }

        private static void OnProjectWindowGUI(string guid, Rect selectionRect)
        {
            if (string.IsNullOrEmpty(guid)) return;

            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path) || !AssetDatabase.IsValidFolder(path)) return;

            // Check dictionary for color
            string folderName = Path.GetFileName(path);
            if (FolderColors.TryGetValue(folderName, out Color targetColor))
            {
                // Safety check: Recreate texture if lost (e.g. after script recompilation)
                if (_gradientTexture == null)
                {
                    GenerateGradientTexture();
                }

                // Logic to avoid drawing over selection if desired
                bool isSelected = Selection.activeObject != null && 
                                  AssetDatabase.GetAssetPath(Selection.activeObject) == path;

                if (!isSelected)
                {
                    // Save current GUI color
                    var originalColor = GUI.color;

                    // Apply folder color tint
                    GUI.color = targetColor;

                    // Draw the texture stretched across the rect
                    // We adjust the rect slightly to add padding or offset if needed
                    Rect drawRect = selectionRect;
                    
                    // Optional: Offset x to start after the expansion arrow if in list mode
                    // usually rect.height is 16 in list view.
                    if (drawRect.height > 20) 
                    {
                        // Icon view (Grid): Draw at bottom or fill? 
                        // Let's fill the bottom label area for grid view
                        drawRect.height = 20;
                        drawRect.y += selectionRect.height - 20;
                    }

                    GUI.DrawTexture(drawRect, _gradientTexture);

                    // Restore original color
                    GUI.color = originalColor;
                }
            }
        }

        /// <summary>
        /// Generates a simple horizontal gradient texture (White Opaque -> Transparent).
        /// </summary>
        private static void GenerateGradientTexture()
        {
            _gradientTexture = new Texture2D(64, 1);
            _gradientTexture.wrapMode = TextureWrapMode.Clamp;
            _gradientTexture.hideFlags = HideFlags.HideAndDontSave;

            for (int i = 0; i < 64; i++)
            {
                // Linear fade from 1 to 0 alpha
                float alpha = 1f - (i / 63f);
                // Apply a slight curve for smoother fade if desired, here linear is clean.
                _gradientTexture.SetPixel(i, 0, new Color(1, 1, 1, alpha));
            }

            _gradientTexture.Apply();
        }
    }
}