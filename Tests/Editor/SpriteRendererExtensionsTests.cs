using NUnit.Framework;
using UnityEngine;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class SpriteRendererExtensionsTests
    {
        [Test]
        public void CSetAlpha_WithValidRenderer_SetsAlpha()
        {
            var go = new GameObject("SR");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.color = Color.white;

            sr.CSetAlpha(0.5f);

            Assert.AreEqual(0.5f, sr.color.a, 0.001f);
            Assert.AreEqual(1f, sr.color.r, 0.001f);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void CSetAlpha_WithNullRenderer_DoesNotThrow()
        {
            SpriteRenderer sr = null;
            Assert.DoesNotThrow(() => sr.CSetAlpha(0.5f));
        }

        [Test]
        public void CSetAlphaUnsafe_WithValidRenderer_SetsAlpha()
        {
            var go = new GameObject("SR");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.color = new Color(0.5f, 0.5f, 0.5f, 1f);

            sr.CSetAlphaUnsafe(0.25f);

            Assert.AreEqual(0.25f, sr.color.a, 0.001f);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void CSetAlpha_ZeroAlpha_MakesTransparent()
        {
            var go = new GameObject("SR");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.color = Color.white;

            sr.CSetAlpha(0f);

            Assert.AreEqual(0f, sr.color.a, 0.001f);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void CSetAlpha_OneAlpha_FullyOpaque()
        {
            var go = new GameObject("SR");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.color = new Color(1, 1, 1, 0);

            sr.CSetAlpha(1f);

            Assert.AreEqual(1f, sr.color.a, 0.001f);
            Object.DestroyImmediate(go);
        }
    }
}
