#if NEWTONSOFT_JSON
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

using Newtonsoft.Json;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NCDK
{
    [JsonObject(MemberSerialization.OptIn)]
    [Serializable]
    public abstract class GameStateBase : PersistentData
    {

        #region <<---------- Initializers ---------->>

        protected GameStateBase(string name = null, bool isAutoInitialized = false)
        {
            this.WasLoadedAutomatically = isAutoInitialized;
            this.SaveIdentifier = SerializableGuid.NewGuid();
            if (!name.IsNullOrEmpty()) this.SaveDescriptiveName = name;
            this.SaveDate = DateTime.UtcNow;
            AppVersionWhenCreated = new Version(Application.version);
        }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Properties and Fields ---------->>

        public static event Action OnNotifyForExternalModifiedSaveFile
        {
            add
            {
                _onNotifyForExternalModifiedSaveFile -= value;
                _onNotifyForExternalModifiedSaveFile += value;
            }
            remove => _onNotifyForExternalModifiedSaveFile -= value;
        }
        [NonSerialized] static Action _onNotifyForExternalModifiedSaveFile;

        /// <summary>
        /// Fired when a save file exists but is corrupted and no backup is recoverable.
        /// A fresh save will be created silently afterward.
        /// </summary>
        public static event Action OnSaveCorrupted;

        [JsonProperty("_appVersionWhenCreated"), SerializeField]
        string _appVersionWhenCreated;
        public Version AppVersionWhenCreated
        {
            get => Version.TryParse(_appVersionWhenCreated, out var version) ? version : default;
            set => _appVersionWhenCreated = value.ToString();
        }

        [JsonProperty("_appVersionOnLastSave"), SerializeField]
        string _appVersionOnLastSave;
        public Version AppVersionOnLastSave
        {
            get => Version.TryParse(_appVersionOnLastSave, out var version) ? version : default;
            set => _appVersionOnLastSave = value.ToString();
        }

        public bool WasLoadedAutomatically { get; }

        public const string SavesDirectoryName = "save";

        [JsonProperty("_saveIdentifier")]
        public SerializableGuid SaveIdentifier;

        [JsonProperty("_saveDescriptiveName")]
        public string SaveDescriptiveName = "Save";

        [JsonProperty("_saveDateTime")]
        public DateTime SaveDate;

        [JsonProperty("_saveHash")]
        public string SaveHash;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Save ---------->>

        public virtual bool Save()
        {
            return this.SaveJson();
        }

        bool SaveJson()
        {
            this.SaveDate = DateTime.UtcNow;
            this.SaveHash = String.Empty;
            if (Version.TryParse(Application.version, out var version))
            {
                this.AppVersionOnLastSave = version;
            }
            // serialized json without hash
            this.SaveHash = Animator.StringToHash(this.GetSerializedJson()).ToString();
            // then serialize again and save with hash and a short name
            var filePath = GetGameStateFilePath(this.SaveIdentifier.ToShortString());
            return SaveJsonTextToFileAtomic(this.GetSerializedJson(), filePath);
        }

        /// <summary>
        /// Atomically writes JSON to file with a backup (.bak) of the previous version.
        /// Writes to .tmp first, then renames to final path.
        /// </summary>
        static bool SaveJsonTextToFileAtomic(string json, string filePath)
        {
            var tempPath = filePath + ".tmp";
            var backupPath = filePath + ".bak";

            try
            {
#if UNITY_EDITOR
                string contentToWrite = json;
#else
                string contentToWrite = EncryptionUtils.Encrypt(json);
                if (string.IsNullOrEmpty(contentToWrite)) return false;
#endif
                // 1. Write to temp file
                using (var streamWriter = File.CreateText(tempPath))
                {
                    streamWriter.Write(contentToWrite);
                }

                // 2. Replace old backup
                if (File.Exists(backupPath))
                    File.Delete(backupPath);

                // 3. Move current file to backup (if exists)
                if (File.Exists(filePath))
                    File.Move(filePath, backupPath);

                // 4. Move temp to final path
                File.Move(tempPath, filePath);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Save] Atomic write failed: {e.Message}");

                // Try to restore from backup
                try
                {
                    if (!File.Exists(filePath) && File.Exists(backupPath))
                        File.Move(backupPath, filePath);
                }
                catch { }

                return false;
            }
            finally
            {
                // Clean up temp file if it still exists
                try { if (File.Exists(tempPath)) File.Delete(tempPath); } catch { }
            }
        }

        string GetSerializedJson()
        {
#if NEWTONSOFT_JSON
            return JsonConvert.SerializeObject(this, JsonExtensions.DefaultSettings);
#else
            return JsonUtility.ToJson(this);
#endif
        }

        #endregion <<---------- Save ---------->>




        #region <<---------- Loading ---------->>

        public static T LoadFromId<T>(string saveFileNameWithoutExtension) where T : PersistentData
        {
            try
            {
                var filePath = GetGameStateFilePath(saveFileNameWithoutExtension);

                Debug.Log($"Trying to LoadGameProgress with file '{filePath}'");

                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"SaveGameProgress file at path '{filePath}' doesn't exist!");
                    return null;
                }

                return LoadFromPath<T>(filePath);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return null;
        }

        public static T LoadFromPath<T>(string filePath) where T : PersistentData
        {
            try
            {
                var fileContent = File.ReadAllText(filePath);
                Debug.Log($"Read {fileContent.Length} characters from save file at '{filePath}'.");
                var jsonContent = EncryptionUtils.Decrypt(fileContent);
                if (string.IsNullOrEmpty(jsonContent))
                {
                    Debug.LogError($"Save file at '{filePath}' is corrupted or could not be decrypted. Trying backup...");
                    return TryLoadBackup<T>(filePath);
                }

                var save = DeserializeFile<T>(jsonContent);
                if (save == null)
                {
                    Debug.LogError($"Could not deserialize Save at path '{filePath}'! Trying backup...");
                    return TryLoadBackup<T>(filePath);
                }

                CheckForModifiedFile(save);

                Debug.Log($"Successfully loaded save file '{Path.GetFileName(filePath)}' from path '{filePath}'.");
                return save;
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}. Trying backup...");
                return TryLoadBackup<T>(filePath);
            }
        }

        static T TryLoadBackup<T>(string filePath) where T : PersistentData
        {
            var backupPath = filePath + ".bak";
            if (!File.Exists(backupPath))
            {
                Debug.LogError($"Backup not found at '{backupPath}'. Save data is lost.");
                OnSaveCorrupted?.Invoke();
                return null;
            }

            Debug.LogWarning($"Attempting to load backup from '{backupPath}'.");
            try
            {
                var backupContent = File.ReadAllText(backupPath);
                var jsonContent = EncryptionUtils.Decrypt(backupContent);
                if (string.IsNullOrEmpty(jsonContent))
                {
                    OnSaveCorrupted?.Invoke();
                    return null;
                }

                var save = DeserializeFile<T>(jsonContent);
                if (save == null)
                {
                    OnSaveCorrupted?.Invoke();
                    return null;
                }

                CheckForModifiedFile(save);
                Debug.LogWarning($"Loaded from backup '{Path.GetFileName(backupPath)}'. Main save was corrupted.");
                return save;
            }
            catch (Exception e)
            {
                Debug.LogError($"Backup also corrupted: {e.Message}");
                OnSaveCorrupted?.Invoke();
                return null;
            }
        }

        static T DeserializeFile<T>(string fileContent) where T : PersistentData
        {
#if NEWTONSOFT_JSON
            return JsonConvert.DeserializeObject<T>(fileContent, JsonExtensions.DefaultSettings);
#else
			return JsonUtility.FromJson<T>(fileContent);
#endif
        }

        private static IEnumerable<T> EnumerateSaveFiles<T>() where T : GameStateBase
        {
            string[] filesPaths;
            try
            {
                var directory = new DirectoryInfo(GetGameStateFolder());
                if (!directory.Exists) yield break;

                // Sort by LastWriteTime descending to get newest first
                filesPaths = directory.GetFiles($"*{EnigmaPaths.SaveExtension}")
                    .OrderByDescending(f => f.LastWriteTime)
                    .Select(f => f.FullName)
                    .ToArray();

                Debug.Log($"Found {filesPaths.Length} save files.");
            }
            catch (Exception e)
            {
                Debug.LogError("Could not access save folder: " + e);
                yield break;
            }

            foreach (var filePath in filesPaths)
            {
                T save = null;
                try
                {
                    save = LoadFromPath<T>(filePath);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error reading file from path '{filePath}': " + e);
                }

                if (save != null)
                {
                    yield return save;
                }
            }
        }

        /// <summary>
        /// Returns all saves ordered by most recent. Never returns a null list.
        /// </summary>
        public static List<T> GetAllSaveFiles<T>() where T : GameStateBase
        {
            return new List<T>(EnumerateSaveFiles<T>());
        }

        /// <summary>
        /// Returns the most recent save file or null if none found.
        /// </summary>
        public static T GetMostRecentSaveFile<T>() where T : GameStateBase
        {
            using var enumerator = EnumerateSaveFiles<T>().GetEnumerator();
            return enumerator.MoveNext() ? enumerator.Current : null;
        }

        #endregion <<---------- Loading ---------->>




        #region Deleting

        public bool DeleteSave()
        {
            try
            {
                var filePath = GetGameStateFilePath(this.SaveIdentifier.ToShortString());
                if (!File.Exists(filePath)) return false;
                File.Delete(filePath);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return false;
        }

        #endregion Deleting




        #region <<---------- Paths ---------->>

        private static string _cachedGameStateFolder;

        public static string GetGameStateFolder()
        {
            if (!string.IsNullOrEmpty(_cachedGameStateFolder)) return _cachedGameStateFolder;

            var persistentDataPath = GetApplicationPersistentDataFolder();

            // Default path: .../NullComma/ApplicationName/save
            var folderPath = Path.Combine(persistentDataPath, SavesDirectoryName);

            // Steamworks support: .../NullComma/ApplicationName/SteamID/save
            ulong? steamId = GetSteamID();
            if (steamId.HasValue)
            {
                folderPath = Path.Combine(persistentDataPath, steamId.Value.ToString(), SavesDirectoryName);
            }

            folderPath = folderPath.Replace('\\', '/');
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            _cachedGameStateFolder = folderPath;
            return folderPath;
        }


        private static ulong? GetSteamID()
        {
            try
            {
                // Use Reflection to avoid hard dependency on Facepunch.Steamworks
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var steamAssembly = assemblies.FirstOrDefault(a => a.GetName().Name.StartsWith("Facepunch.Steamworks"));

                if (steamAssembly != null)
                {
                    var clientType = steamAssembly.GetType("Steamworks.SteamClient");
                    if (clientType != null)
                    {
                        var isValidProp = clientType.GetProperty("IsValid", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        if (isValidProp != null && (bool)isValidProp.GetValue(null))
                        {
                            var steamIdProp = clientType.GetProperty("SteamId", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                            var steamIdObj = steamIdProp.GetValue(null);
                            // SteamId is a struct with a 'Value' ulong property
                            var valueProp = steamIdObj.GetType().GetProperty("Value", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                            if (valueProp != null)
                            {
                                return (ulong)valueProp.GetValue(steamIdObj);
                            }
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public static string GetGameStateFilePath(string fileName)
        {
            return Path.Combine(GetGameStateFolder(), $"{fileName}{EnigmaPaths.SaveExtension}");
        }

        #endregion <<---------- Paths ---------->>




        #region <<---------- General ---------->>

        public static bool CheckForModifiedFile<T>(T dataT)
        {
            if (!(dataT is GameStateBase data)) return true;
            var originalHash = data.SaveHash;
            data.SaveHash = string.Empty;
            var newHash = Animator.StringToHash(data.GetSerializedJson()).ToString();
            if (originalHash != newHash)
            {
                Debug.Log($"Save file '{data.SaveIdentifier}' was modified externally!");
                _onNotifyForExternalModifiedSaveFile?.Invoke();
                return true;
            }
            data.SaveHash = originalHash;
            return false;
        }

        #endregion <<---------- General ---------->>

    }
}
#endif
