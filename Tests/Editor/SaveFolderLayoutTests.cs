using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

namespace NCDK.Tests.Editor
{
    /// <summary>
    /// Regression cover for the save folder layout: Steam Cloud syncs
    /// .../NullComma/ApplicationName/save, so the account id has to live inside that folder.
    /// </summary>
    [TestFixture]
    public class SaveFolderLayoutTests
    {
        [Test]
        public void GetGameStateFolder_IsInsideTheCloudSyncedSaveFolder()
        {
            string folder = GameStateBase.GetGameStateFolder().Replace('\\', '/');
            string persistent = Application.persistentDataPath.Replace('\\', '/');
            string cloudRoot = persistent + "/save";

            Assert.IsTrue(
                folder == cloudRoot || Regex.IsMatch(folder, Regex.Escape(cloudRoot) + @"/\d+$"),
                $"expected '{cloudRoot}' or '{cloudRoot}/<steamid>', got '{folder}'");
        }

        [Test]
        public void GetGameStateFolder_DoesNotPutTheAccountIdAboveTheSaveFolder()
        {
            string folder = GameStateBase.GetGameStateFolder().Replace('\\', '/');
            string persistent = Application.persistentDataPath.Replace('\\', '/');

            // The old bug wrote to <persistent>/<steamid>/save, which Steam Cloud never synced.
            Assert.IsFalse(
                Regex.IsMatch(folder, "^" + Regex.Escape(persistent) + @"/\d+/save$"),
                $"the account id must sit inside the save folder, got '{folder}'");
        }

        [Test]
        public void ResetGameStateFolderCache_AllowsTheFolderToBeResolvedAgain()
        {
            string first = GameStateBase.GetGameStateFolder();
            GameStateBase.ResetGameStateFolderCache();
            string second = GameStateBase.GetGameStateFolder();

            Assert.AreEqual(first, second, "re-resolving must land on the same folder in the same session");
        }
    }
}
