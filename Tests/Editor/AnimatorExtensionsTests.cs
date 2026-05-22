using NUnit.Framework;
using UnityEngine;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class AnimatorExtensionsTests
    {
        private GameObject _go;
        private Animator _animator;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject("AnimatorTest");
            _animator = _go.AddComponent<Animator>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_go);
        }

        [Test]
        public void SetBoolSafe_WithNullAnimator_DoesNotThrow()
        {
            Animator nullAnim = null;
            Assert.DoesNotThrow(() => nullAnim.SetBoolSafe(0, true));
        }

        [Test]
        public void SetFloatSafe_WithNullAnimator_DoesNotThrow()
        {
            Animator nullAnim = null;
            Assert.DoesNotThrow(() => nullAnim.SetFloatSafe(0, 1f));
        }

        [Test]
        public void SetIntegerSafe_WithNullAnimator_DoesNotThrow()
        {
            Animator nullAnim = null;
            Assert.DoesNotThrow(() => nullAnim.SetIntegerSafe(0, 5));
        }

        [Test]
        public void SetTriggerSafe_WithNullAnimator_DoesNotThrow()
        {
            Animator nullAnim = null;
            Assert.DoesNotThrow(() => nullAnim.SetTriggerSafe(0));
        }

        [Test]
        public void SetFloatWithLerp_WithNullAnimator_DoesNotThrow()
        {
            Animator nullAnim = null;
            Assert.DoesNotThrow(() => nullAnim.SetFloatWithLerp(0, 1f, 0.5f));
        }

        [Test]
        public void SetBoolSafe_WithNonexistentParam_DoesNotThrow()
        {
            int fakeHash = Animator.StringToHash("NONEXISTENT_PARAM_XYZ");
            Assert.DoesNotThrow(() => _animator.SetBoolSafe(fakeHash, true));
        }

        [Test]
        public void SetFloatSafe_WithNonexistentParam_DoesNotThrow()
        {
            int fakeHash = Animator.StringToHash("NONEXISTENT_PARAM_XYZ");
            Assert.DoesNotThrow(() => _animator.SetFloatSafe(fakeHash, 1f));
        }

        [Test]
        public void SetIntegerSafe_WithNonexistentParam_DoesNotThrow()
        {
            int fakeHash = Animator.StringToHash("NONEXISTENT_PARAM_XYZ");
            Assert.DoesNotThrow(() => _animator.SetIntegerSafe(fakeHash, 5));
        }

        [Test]
        public void SetTriggerSafe_WithNonexistentParam_DoesNotThrow()
        {
            int fakeHash = Animator.StringToHash("NONEXISTENT_PARAM_XYZ");
            Assert.DoesNotThrow(() => _animator.SetTriggerSafe(fakeHash));
        }
    }
}
