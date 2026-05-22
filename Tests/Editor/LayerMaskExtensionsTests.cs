using NUnit.Framework;
using UnityEngine;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class LayerMaskExtensionsTests
    {
        [Test]
        public void CContains_WithLayerInMask_ReturnsTrue()
        {
            int layer = LayerMask.NameToLayer("Default");
            LayerMask mask = 1 << layer;
            Assert.IsTrue(mask.CContains(layer));
        }

        [Test]
        public void CContains_WithLayerNotInMask_ReturnsFalse()
        {
            int layer1 = LayerMask.NameToLayer("Default");
            int layer2 = LayerMask.NameToLayer("Ignore Raycast");
            if (layer1 == layer2) Assert.Ignore("Layers are the same");
            LayerMask mask = 1 << layer1;
            Assert.IsFalse(mask.CContains(layer2));
        }

        [Test]
        public void CContains_WithZeroMask_ReturnsFalse()
        {
            LayerMask mask = 0;
            int layer = LayerMask.NameToLayer("Default");
            Assert.IsFalse(mask.CContains(layer));
        }

        [Test]
        public void CContains_WithAllLayers_ReturnsTrue()
        {
            LayerMask mask = ~0;
            int layer = LayerMask.NameToLayer("Default");
            Assert.IsTrue(mask.CContains(layer));
        }
    }
}
