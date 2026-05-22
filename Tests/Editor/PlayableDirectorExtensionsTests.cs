using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class PlayableDirectorExtensionsTests
    {
        [Test]
        public void SetAsActiveAndPlay_WithNull_ReturnsNull()
        {
            PlayableDirector pd = null;
            Assert.IsNull(pd.SetAsActiveAndPlay());
        }

        [Test]
        public void IsPlaying_WithNull_ReturnsFalse()
        {
            PlayableDirector pd = null;
            Assert.IsFalse(pd.IsPlaying());
        }

        [Test]
        public void IsPlaying_WithNewDirector_ReturnsFalse()
        {
            var go = new GameObject("PD");
            var pd = go.AddComponent<PlayableDirector>();
            Assert.IsFalse(pd.IsPlaying());
            Object.DestroyImmediate(go);
        }

        [Test]
        public void SetAsActiveAndPlay_WithDirector_ReturnsSame()
        {
            var go = new GameObject("PD");
            var pd = go.AddComponent<PlayableDirector>();
            Assert.AreEqual(pd, pd.SetAsActiveAndPlay());
            Object.DestroyImmediate(go);
        }
    }
}
