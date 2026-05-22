using NUnit.Framework;
using UnityEngine;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class AvatarIKGoalExtensionsTests
    {
        [Test]
        public void IsFoot_LeftFoot_ReturnsTrue()
        {
            Assert.IsTrue(AvatarIKGoal.LeftFoot.IsFoot());
        }

        [Test]
        public void IsFoot_RightFoot_ReturnsTrue()
        {
            Assert.IsTrue(AvatarIKGoal.RightFoot.IsFoot());
        }

        [Test]
        public void IsFoot_LeftHand_ReturnsFalse()
        {
            Assert.IsFalse(AvatarIKGoal.LeftHand.IsFoot());
        }

        [Test]
        public void IsHand_LeftHand_ReturnsTrue()
        {
            Assert.IsTrue(AvatarIKGoal.LeftHand.IsHand());
        }

        [Test]
        public void IsHand_RightHand_ReturnsTrue()
        {
            Assert.IsTrue(AvatarIKGoal.RightHand.IsHand());
        }

        [Test]
        public void IsHand_LeftFoot_ReturnsFalse()
        {
            Assert.IsFalse(AvatarIKGoal.LeftFoot.IsHand());
        }
    }
}
