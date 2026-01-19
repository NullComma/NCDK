using NUnit.Framework;
using UnityEngine;

namespace EnigmaCore.Tests.Editor
{
    [TestFixture]
    public class ReflectionHelpersTests
    {
        private class TestClass
        {
            private int privateField = 42;
            public int publicField = 100;
            private string privateProperty { get; set; } = "private";
            public string PublicProperty { get; set; } = "public";
            
            private void PrivateMethod() { }
            public void PublicMethod() { }
            
            private int PrivateMethodWithReturn() => 999;
            public int PublicMethodWithReturn() => 888;
            
            private string PrivateMethodWithParams(int a, string b) => $"{a}_{b}";
        }

        [Test]
        public void GetPrivateField_WithValidField_ReturnsValue()
        {
            var obj = new TestClass();
            int value = obj.GetPrivateField<int>("privateField");
            Assert.AreEqual(42, value);
        }

        [Test]
        public void GetPrivateField_WithPublicField_ReturnsValue()
        {
            var obj = new TestClass();
            int value = obj.GetPrivateField<int>("publicField");
            Assert.AreEqual(100, value);
        }

        [Test]
        public void GetPrivateField_WithInvalidField_ThrowsArgumentException()
        {
            var obj = new TestClass();
            Assert.Throws<System.ArgumentException>(() => obj.GetPrivateField<int>("nonExistentField"));
        }

        [Test]
        public void SetPrivateField_WithValidField_SetsValue()
        {
            var obj = new TestClass();
            obj.SetPrivateField("privateField", 99);
            int value = obj.GetPrivateField<int>("privateField");
            Assert.AreEqual(99, value);
        }

        [Test]
        public void GetPrivateProperty_WithValidProperty_ReturnsValue()
        {
            var obj = new TestClass();
            string value = obj.GetPrivateProperty<string>("privateProperty");
            Assert.AreEqual("private", value);
        }

        [Test]
        public void GetPrivateProperty_WithPublicProperty_ReturnsValue()
        {
            var obj = new TestClass();
            string value = obj.GetPrivateProperty<string>("PublicProperty");
            Assert.AreEqual("public", value);
        }

        [Test]
        public void GetPrivateProperty_WithInvalidProperty_ThrowsArgumentException()
        {
            var obj = new TestClass();
            Assert.Throws<System.ArgumentException>(() => obj.GetPrivateProperty<string>("NonExistentProperty"));
        }

        [Test]
        public void SetPrivateProperty_WithValidProperty_SetsValue()
        {
            var obj = new TestClass();
            obj.SetPrivateProperty("privateProperty", "updated");
            string value = obj.GetPrivateProperty<string>("privateProperty");
            Assert.AreEqual("updated", value);
        }

        [Test]
        public void InvokePrivateMethod_WithValidMethod_Executes()
        {
            var obj = new TestClass();
            Assert.DoesNotThrow(() => obj.InvokePrivateMethod("PrivateMethod"));
        }

        [Test]
        public void InvokePrivateMethod_WithValidMethodWithReturn_ReturnsValue()
        {
            var obj = new TestClass();
            int result = obj.InvokePrivateMethod<int>("PrivateMethodWithReturn");
            Assert.AreEqual(999, result);
        }

        [Test]
        public void InvokePrivateMethod_WithValidMethodWithParams_Works()
        {
            var obj = new TestClass();
            string result = obj.InvokePrivateMethod<string>("PrivateMethodWithParams", 42, "test");
            Assert.AreEqual("42_test", result);
        }

        private class InheritedClass : TestClass
        {
            private int basePrivateField = 200;
        }

        [Test]
        public void GetPrivateField_WithInheritedClass_AccessesBaseClassField()
        {
            var obj = new InheritedClass();
            int value = obj.GetPrivateField<int>("privateField");
            Assert.AreEqual(42, value);
        }

        [Test]
        public void GetPrivateField_WithInheritedClass_AccessesDerivedClassField()
        {
            var obj = new InheritedClass();
            int value = obj.GetPrivateField<int>("basePrivateField");
            Assert.AreEqual(200, value);
        }
    }
}
