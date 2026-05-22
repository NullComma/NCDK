using NUnit.Framework;
using UnityEngine;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class ArrayExtensionsTests
    {
        [Test]
        public void DoForEachNotNull_WithNonNullElements_InvokesAction()
        {
            int count = 0;
            GameObject[] array = { new GameObject("a"), new GameObject("b"), new GameObject("c") };
            array.DoForEachNotNull(x => count++);
            Assert.AreEqual(3, count);
            foreach (var go in array) Object.DestroyImmediate(go);
        }

        [Test]
        public void DoForEachNotNull_WithNullElements_SkipsNulls()
        {
            int count = 0;
            GameObject go1 = new GameObject("a");
            GameObject go3 = new GameObject("b");
            GameObject[] array = { go1, null, go3, null, new GameObject("c") };
            array.DoForEachNotNull(x => count++);
            Assert.AreEqual(3, count);
            Object.DestroyImmediate(go1);
            Object.DestroyImmediate(go3);
            Object.DestroyImmediate(array[4]);
        }

        [Test]
        public void DoForEachNotNull_WithEmptyArray_DoesNotInvoke()
        {
            int count = 0;
            GameObject[] array = { };
            array.DoForEachNotNull(x => count++);
            Assert.AreEqual(0, count);
        }

        [Test]
        public void DoForEachNotNull_WithAllNullArray_DoesNotInvoke()
        {
            int count = 0;
            GameObject[] array = { null, null, null };
            array.DoForEachNotNull(x => count++);
            Assert.AreEqual(0, count);
        }
    }
}
