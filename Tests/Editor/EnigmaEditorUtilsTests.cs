#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;

namespace EnigmaCore.Tests.Editor
{
    [TestFixture]
    public class EnigmaEditorUtilsTests
    {
        private class TestObjectWithField
        {
            public bool boolField = true;
            public int intField = 42;
            public float floatField = 3.14f;
        }

        private class TestObjectWithProperty
        {
            public bool BoolProperty { get; set; } = false;
            public int IntProperty { get; private set; } = 100;
            public float FloatProperty { get; } = 2.71f;
        }

        private class TestObjectWithMethod
        {
            public bool BoolMethod() => true;
            public int IntMethod() => 200;
        }

        [Test]
        public void EvaluateCondition_WithBoolField_ReturnsFieldValue()
        {
            var obj = new TestObjectWithField { boolField = true };
            Assert.IsTrue(EnigmaEditorUtils.EvaluateCondition(obj, "boolField"));
            
            obj.boolField = false;
            Assert.IsFalse(EnigmaEditorUtils.EvaluateCondition(obj, "boolField"));
        }

        [Test]
        public void EvaluateCondition_WithBoolProperty_ReturnsPropertyValue()
        {
            var obj = new TestObjectWithProperty { BoolProperty = true };
            Assert.IsTrue(EnigmaEditorUtils.EvaluateCondition(obj, "BoolProperty"));
            
            obj.BoolProperty = false;
            Assert.IsFalse(EnigmaEditorUtils.EvaluateCondition(obj, "BoolProperty"));
        }

        [Test]
        public void EvaluateCondition_WithBoolMethod_ReturnsMethodResult()
        {
            var obj = new TestObjectWithMethod();
            Assert.IsTrue(EnigmaEditorUtils.EvaluateCondition(obj, "BoolMethod"));
        }

        [Test]
        public void EvaluateCondition_WithNullTarget_ReturnsTrue()
        {
            Assert.IsTrue(EnigmaEditorUtils.EvaluateCondition(null, "anything"));
        }

        [Test]
        public void EvaluateCondition_WithEmptyMemberName_ReturnsTrue()
        {
            var obj = new TestObjectWithField();
            Assert.IsTrue(EnigmaEditorUtils.EvaluateCondition(obj, string.Empty));
            Assert.IsTrue(EnigmaEditorUtils.EvaluateCondition(obj, null));
        }

        [Test]
        public void EvaluateCondition_WithNonExistentMember_ReturnsTrue()
        {
            var obj = new TestObjectWithField();
            Assert.IsTrue(EnigmaEditorUtils.EvaluateCondition(obj, "NonExistentMember"));
        }

        [Test]
        public void GetValueFromMember_WithIntField_ReturnsValue()
        {
            var obj = new TestObjectWithField { intField = 123 };
            float value = EnigmaEditorUtils.GetValueFromMember(obj, "intField", 0f);
            Assert.AreEqual(123f, value);
        }

        [Test]
        public void GetValueFromMember_WithFloatField_ReturnsValue()
        {
            var obj = new TestObjectWithField { floatField = 5.5f };
            float value = EnigmaEditorUtils.GetValueFromMember(obj, "floatField", 0f);
            Assert.AreEqual(5.5f, value);
        }

        [Test]
        public void GetValueFromMember_WithIntProperty_ReturnsValue()
        {
            var obj = new TestObjectWithProperty();
            float value = EnigmaEditorUtils.GetValueFromMember(obj, "IntProperty", 0f);
            Assert.AreEqual(100f, value);
        }

        [Test]
        public void GetValueFromMember_WithFloatProperty_ReturnsValue()
        {
            var obj = new TestObjectWithProperty();
            float value = EnigmaEditorUtils.GetValueFromMember(obj, "FloatProperty", 0f);
            Assert.AreEqual(2.71f, value);
        }

        [Test]
        public void GetValueFromMember_WithNonExistentMember_ReturnsDefault()
        {
            var obj = new TestObjectWithField();
            float value = EnigmaEditorUtils.GetValueFromMember(obj, "NonExistent", 999f);
            Assert.AreEqual(999f, value);
        }

        [Test]
        public void GetValueFromMember_WithNullTarget_ReturnsDefault()
        {
            float value = EnigmaEditorUtils.GetValueFromMember(null, "anything", 888f);
            Assert.AreEqual(888f, value);
        }

        [Test]
        public void GetValueFromMember_WithEmptyMemberName_ReturnsDefault()
        {
            var obj = new TestObjectWithField();
            float value = EnigmaEditorUtils.GetValueFromMember(obj, string.Empty, 777f);
            Assert.AreEqual(777f, value);
            value = EnigmaEditorUtils.GetValueFromMember(obj, null, 777f);
            Assert.AreEqual(777f, value);
        }
    }
}
#endif
