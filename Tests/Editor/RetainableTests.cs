using NUnit.Framework;
using UnityEngine;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class RetainableTests
    {
        [Test]
        public void IsRetained_InitiallyFalse()
        {
            var retainable = new Retainable();
            Assert.IsFalse(retainable.IsRetained);
        }

        [Test]
        public void Retain_ReturnsNonNullDisposable()
        {
            var retainable = new Retainable();
            var source = new object();
            var token = retainable.Retain(source);
            Assert.IsNotNull(token);
            token.Dispose();
        }

        [Test]
        public void Retain_SetsIsRetainedTrue()
        {
            var retainable = new Retainable();
            var source = new object();
            retainable.Retain(source);
            Assert.IsTrue(retainable.IsRetained);
        }

        [Test]
        public void Release_SetsIsRetainedFalse()
        {
            var retainable = new Retainable();
            var source = new object();
            retainable.Retain(source);
            retainable.Release(source);
            Assert.IsFalse(retainable.IsRetained);
        }

        [Test]
        public void Retain_SameSourceTwice_ReturnsEmptyDisposable()
        {
            var retainable = new Retainable();
            var source = new object();
            retainable.Retain(source);
            var secondToken = retainable.Retain(source);
            Assert.AreSame(Disposables.Empty, secondToken);
        }

        [Test]
        public void Retain_SameSourceTwice_IsRetainedStaysTrue()
        {
            var retainable = new Retainable();
            var source = new object();
            retainable.Retain(source);
            retainable.Retain(source);
            Assert.IsTrue(retainable.IsRetained);
        }

        [Test]
        public void Release_UnknownSource_DoesNotThrow()
        {
            var retainable = new Retainable();
            Assert.DoesNotThrow(() => retainable.Release(new object()));
        }

        [Test]
        public void Release_UnknownSource_IsRetainedStaysFalse()
        {
            var retainable = new Retainable();
            retainable.Release(new object());
            Assert.IsFalse(retainable.IsRetained);
        }

        [Test]
        public void StateEvent_FiresOnFirstRetain()
        {
            var retainable = new Retainable();
            bool eventFired = false;
            bool eventValue = false;
            retainable.StateEvent += (val) =>
            {
                eventFired = true;
                eventValue = val;
            };
            retainable.Retain(new object());
            Assert.IsTrue(eventFired);
            Assert.IsTrue(eventValue);
        }

        [Test]
        public void StateEvent_FiresOnLastRelease()
        {
            var retainable = new Retainable();
            var source = new object();
            retainable.Retain(source);
            bool eventFired = false;
            bool eventValue = true;
            retainable.StateEvent += (val) =>
            {
                eventFired = true;
                eventValue = val;
            };
            retainable.Release(source);
            Assert.IsTrue(eventFired);
            Assert.IsFalse(eventValue);
        }

        [Test]
        public void StateEvent_DoesNotFireForDuplicateRetain()
        {
            var retainable = new Retainable();
            var source = new object();
            retainable.Retain(source);
            int fireCount = 0;
            retainable.StateEvent += (val) => fireCount++;
            retainable.Retain(source);
            Assert.AreEqual(0, fireCount);
        }

        [Test]
        public void ReleaseHelper_Dispose_ReleasesSource()
        {
            var retainable = new Retainable();
            var source = new object();
            var token = retainable.Retain(source);
            Assert.IsTrue(retainable.IsRetained);
            token.Dispose();
            Assert.IsFalse(retainable.IsRetained);
        }

        [Test]
        public void ReleaseHelper_DisposeMultipleTimes_Idempotent()
        {
            var retainable = new Retainable();
            var source = new object();
            var token = retainable.Retain(source);
            token.Dispose();
            Assert.DoesNotThrow(() => token.Dispose());
            token.Dispose();
            Assert.IsFalse(retainable.IsRetained);
        }

        [Test]
        public void MultipleSources_AllMustRelease()
        {
            var retainable = new Retainable();
            var sourceA = new object();
            var sourceB = new object();
            retainable.Retain(sourceA);
            retainable.Retain(sourceB);
            Assert.IsTrue(retainable.IsRetained);
            retainable.Release(sourceA);
            Assert.IsTrue(retainable.IsRetained);
            retainable.Release(sourceB);
            Assert.IsFalse(retainable.IsRetained);
        }

        [Test]
        public void MultipleSources_ReleaseInReverseOrder()
        {
            var retainable = new Retainable();
            var sourceA = new object();
            var sourceB = new object();
            retainable.Retain(sourceA);
            retainable.Retain(sourceB);
            retainable.Release(sourceB);
            Assert.IsTrue(retainable.IsRetained);
            retainable.Release(sourceA);
            Assert.IsFalse(retainable.IsRetained);
        }

        [Test]
        public void DestroyedUnityObject_CleanedUpOnIsRetainedCheck()
        {
            var retainable = new Retainable();
            var go = new GameObject("RetainTestGO");
            retainable.Retain(go);
            Assert.IsTrue(retainable.IsRetained);
            Object.DestroyImmediate(go);
            Assert.IsFalse(retainable.IsRetained);
        }

        [Test]
        public void CleanUnityObject_IsRetainedTrue()
        {
            var retainable = new Retainable();
            var go = new GameObject("RetainTestGO");
            retainable.Retain(go);
            Assert.IsTrue(retainable.IsRetained);
            Object.DestroyImmediate(go);
        }
    }
}
