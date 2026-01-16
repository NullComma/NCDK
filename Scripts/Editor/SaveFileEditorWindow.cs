using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace EnigmaCore.Editor
{
    public class SaveFileEditorWindow : EditorWindow
    {
        [MenuItem(StaticStrings.PrefixTools + "Save File Editor")]
        public static void ShowWindow()
        {
            GetWindow<SaveFileEditorWindow>("Save Editor");
        }

        private List<string> _saveFiles = new List<string>();
        private string _selectedFilePath;
        private JObject _currentJson;
        private Vector2 _scrollPos;
        private Vector2 _fileListScrollPos;
        private string _statusMessage;

        private void OnEnable()
        {
            ReloadFiles();
        }

        private void ReloadFiles()
        {
            _saveFiles.Clear();
            _currentJson = null;
            _selectedFilePath = null;
            _statusMessage = string.Empty;

            try
            {
                // We rely on CPersistentData to tell us the folder, 
                // but we need to verify if the method is accessible or we just reconstruct the path.
                // Looking at the provided context, GetGameStateFolder is public static.
                string folderPath = GetGameStateFolderPath();
                if (Directory.Exists(folderPath))
                {
                    _saveFiles = Directory.GetFiles(folderPath, $"*{EnigmaPaths.SaveExtension}")
                        .OrderByDescending(File.GetLastWriteTime)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error listing save files: {e.Message}");
            }
        }

        // Helper to get folder path, replicating CPersistentData logic if needed, 
        // but since we are in the same assembly assembly definition (usually) or Editor folder, 
        // we can try to access it. CPersistentData is in EnigmaCore namespace.
        // If CPersistentData is not accessible due to asmdef, we might need a fallback.
        // Assuming Editor folder is in an assembly that references the runtime assembly.
        private string GetGameStateFolderPath()
        {
            return CGameStateBase.GetGameStateFolder();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("Refresh Files", EditorStyles.toolbarButton))
            {
                ReloadFiles();
            }
            if (GUILayout.Button("Save Changes", EditorStyles.toolbarButton))
            {
                SaveChanges();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            // Left Sidebar: File List
            EditorGUILayout.BeginVertical(GUILayout.Width(250), GUILayout.ExpandHeight(true));
            DrawSidebar();
            EditorGUILayout.EndVertical();

            // Separator
            GUILayout.Box("", GUILayout.Width(1), GUILayout.ExpandHeight(true));

            // Right Side: Editor
            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            DrawMainEditor();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSidebar()
        {
            EditorGUILayout.LabelField("Save Files", EditorStyles.boldLabel);
            _fileListScrollPos = EditorGUILayout.BeginScrollView(_fileListScrollPos);

            if (_saveFiles.Count == 0)
            {
                EditorGUILayout.HelpBox("No save files found.", MessageType.Info);
            }

            foreach (string file in _saveFiles)
            {
                string fileName = Path.GetFileName(file);
                bool isSelected = _selectedFilePath == file;

                GUIStyle style = new GUIStyle(EditorStyles.label);
                if (isSelected)
                {
                    style.normal.textColor = Color.cyan;
                    style.fontStyle = FontStyle.Bold;
                }

                if (GUILayout.Button(fileName, style))
                {
                    if (!isSelected)
                    {
                        LoadFile(file);
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawMainEditor()
        {
            if (string.IsNullOrEmpty(_selectedFilePath))
            {
                EditorGUILayout.HelpBox("Select a save file to edit.", MessageType.Info);
                return;
            }

            if (_currentJson == null)
            {
                EditorGUILayout.HelpBox($"Could not load or decrypt file:\n{_selectedFilePath}", MessageType.Error);
                return;
            }

            if (!string.IsNullOrEmpty(_statusMessage))
            {
                EditorGUILayout.HelpBox(_statusMessage, MessageType.Info);
            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            
            EditorGUILayout.LabelField("JSON Content", EditorStyles.boldLabel);
            DrawJObject(_currentJson);

            EditorGUILayout.EndScrollView();
        }

        private void LoadFile(string path)
        {
            _selectedFilePath = path;
            _statusMessage = string.Empty;
            try
            {
                string fileContent = File.ReadAllText(path);
                string decrypted = EncryptionUtils.Decrypt(fileContent);

                if (string.IsNullOrEmpty(decrypted))
                {
                    Debug.LogError("Failed to decrypt file or file is empty.");
                    _currentJson = null;
                    return;
                }

                _currentJson = JObject.Parse(decrypted);
                _statusMessage = "File loaded and decrypted successfully.";
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading file: {e.Message}");
                _currentJson = null;
            }
        }

        private void SaveChanges()
        {
            if (_currentJson == null || string.IsNullOrEmpty(_selectedFilePath)) return;

            try
            {
                // 1. Reset Hash
                if (_currentJson.ContainsKey("_saveHash"))
                {
                    _currentJson["_saveHash"] = string.Empty;
                }

                // 2. Serialize to get content for hashing
                // CJsonExtensions.DefaultSettings uses Formatting.Indented
                string jsonForHash = _currentJson.ToString(Formatting.Indented);

                // 3. Calculate Hash
                int hash = Animator.StringToHash(jsonForHash);
                _currentJson["_saveHash"] = hash.ToString();

                // 4. Final Serialize
                string finalJson = _currentJson.ToString(Formatting.Indented);

                // 5. Encrypt
                string encrypted = EncryptionUtils.Encrypt(finalJson);

                // 6. Write to File
                File.WriteAllText(_selectedFilePath, encrypted);
                
                _statusMessage = $"File saved successfully at {DateTime.Now.ToShortTimeString()}";
                ReloadFiles();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving file: {e.Message}");
                EditorUtility.DisplayDialog("Error", $"Could not save file:\n{e.Message}", "OK");
            }
        }

        // --- Recursive JSON Drawing ---

        private void DrawJToken(JToken token, string label = null)
        {
            if (token == null) return;

            string displayLabel = label ?? token.Path.Split('.').LastOrDefault() ?? "Root";

            switch (token.Type)
            {
                case JTokenType.Object:
                    if (token is JObject obj)
                    {
                        if (IsSerializableGuid(obj))
                        {
                            DrawSerializableGuid(obj, displayLabel);
                        }
                        else
                        {
                            DrawJObject(obj, displayLabel);
                        }
                    }
                    break;
                case JTokenType.Array:
                    if (token is JArray arr)
                    {
                        DrawJArray(arr, displayLabel);
                    }
                    else
                    {
                        EditorGUILayout.LabelField(displayLabel, "Unknown Array Type");
                    }
                    break;
                case JTokenType.Integer:
                    {
                        JValue val = (JValue)token;
                        long current = Convert.ToInt64(val.Value);
                        long newVal = EditorGUILayout.LongField(displayLabel, current);
                        if (newVal != current) val.Value = newVal;
                    }
                    break;
                case JTokenType.Float:
                    {
                        JValue val = (JValue)token;
                        float current = Convert.ToSingle(val.Value);
                        float newVal = EditorGUILayout.FloatField(displayLabel, current);
                        if (Math.Abs(newVal - current) > float.Epsilon) val.Value = newVal;
                    }
                    break;
                case JTokenType.String:
                    {
                        JValue val = (JValue)token;
                        string current = (string)val.Value;
                        string newVal = EditorGUILayout.TextField(displayLabel, current);
                        if (newVal != current) val.Value = newVal;
                    }
                    break;
                case JTokenType.Boolean:
                    {
                        JValue val = (JValue)token;
                        bool current = Convert.ToBoolean(val.Value);
                        bool newVal = EditorGUILayout.Toggle(displayLabel, current);
                        if (newVal != current) val.Value = newVal;
                    }
                    break;
                case JTokenType.Date:
                    {
                        JValue val = (JValue)token;
                        string current = val.Value?.ToString() ?? "";
                        EditorGUILayout.LabelField(displayLabel, current + " (Date - ReadOnly in Editor)");
                    }
                    break;
                default:
                    EditorGUILayout.LabelField(displayLabel, $"unsupported type: {token.Type}");
                    break;
            }
        }

        private void DrawJObject(JObject obj, string label = null)
        {
            if (string.IsNullOrEmpty(label))
            {
                foreach (var property in obj.Properties())
                {
                    DrawJToken(property.Value, property.Name);
                }
            }
            else
            {
                EditorGUI.indentLevel++;
                bool unfold = EditorGUILayout.Foldout(true, label);
                if (unfold)
                {
                    foreach (var property in obj.Properties())
                    {
                        DrawJToken(property.Value, property.Name);
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        private void DrawJArray(JArray arr, string label)
        {
            EditorGUI.indentLevel++;
            bool unfold = EditorGUILayout.Foldout(true, $"{label} [{arr.Count}]");
            if (unfold)
            {
                int indexToRemove = -1;
                for (int i = 0; i < arr.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    
                    // Use a vertical group to ensure DrawJToken's layout (which might be multi-line) handles correctly
                    EditorGUILayout.BeginVertical();
                    DrawJToken(arr[i], $"[{i}]");
                    EditorGUILayout.EndVertical();

                    // Remove button
                    if (GUILayout.Button("-", GUILayout.Width(25), GUILayout.Height(EditorStyles.toolbarButton.fixedHeight)))
                    {
                        indexToRemove = i;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (indexToRemove >= 0)
                {
                    arr.RemoveAt(indexToRemove);
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("+ Add Element", GUILayout.Width(150)))
                {
                    if (arr.Count > 0)
                    {
                        // Clone the last element to preserve structure
                        arr.Add(arr.Last.DeepClone());
                    }
                    else
                    {
                        // Fallback: Add empty Object. Users can change it if we added type selection, 
                        // but for now JObject is the most safe default for game saves.
                        arr.Add(new JObject());
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }
        private bool IsSerializableGuid(JObject obj)
        {
            return obj.Count == 4 &&
                   obj.ContainsKey("Part1") &&
                   obj.ContainsKey("Part2") &&
                   obj.ContainsKey("Part3") &&
                   obj.ContainsKey("Part4");
        }

        private void DrawSerializableGuid(JObject obj, string label)
        {
            try
            {
                // Read parts
                uint p1 = (uint)obj["Part1"];
                uint p2 = (uint)obj["Part2"];
                uint p3 = (uint)obj["Part3"];
                uint p4 = (uint)obj["Part4"];

                // Reconstruct Guid
                byte[] bytes = new byte[16];
                BitConverter.GetBytes(p1).CopyTo(bytes, 0);
                BitConverter.GetBytes(p2).CopyTo(bytes, 4);
                BitConverter.GetBytes(p3).CopyTo(bytes, 8);
                BitConverter.GetBytes(p4).CopyTo(bytes, 12);
                Guid guid = new Guid(bytes);

                // Draw
                string currentStr = guid.ToString();
                string newStr = EditorGUILayout.TextField(label, currentStr);

                // Update if changed
                if (newStr != currentStr && Guid.TryParse(newStr, out Guid newGuid))
                {
                    byte[] newBytes = newGuid.ToByteArray();
                    obj["Part1"] = BitConverter.ToUInt32(newBytes, 0);
                    obj["Part2"] = BitConverter.ToUInt32(newBytes, 4);
                    obj["Part3"] = BitConverter.ToUInt32(newBytes, 8);
                    obj["Part4"] = BitConverter.ToUInt32(newBytes, 12);
                }
            }
            catch (Exception)
            {
                // Fallback if something is wrong with the data
                DrawJObject(obj, label);
            }
        }
    }
}
