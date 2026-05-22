using System;
using NUnit.Framework;
using System.Collections.Generic;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class ReactivePropertyTests
    {
        [Test]
        public void Constructor_WithInitialValue_SetsValue()
        {
            var prop = new ReactiveProperty<int>(42);
            Assert.AreEqual(42, prop.Value);
        }

        [Test]
        public void Constructor_WithoutValue_SetsDefault()
        {
            var prop = new ReactiveProperty<int>();
            Assert.AreEqual(0, prop.Value);
        }

        [Test]
        public void Constructor_ReferenceType_DefaultsToNull()
        {
            var prop = new ReactiveProperty<string>();
            Assert.IsNull(prop.Value);
        }

        [Test]
        public void SetValue_DifferentValue_FiresOnValueChanged()
        {
            var prop = new ReactiveProperty<int>(0);
            bool fired = false;
            prop.OnValueChanged += _ => fired = true;

            prop.Value = 10;

            Assert.IsTrue(fired);
        }

        [Test]
        public void SetValue_SameValue_DoesNotFireOnValueChanged()
        {
            var prop = new ReactiveProperty<int>(5);
            bool fired = false;
            prop.OnValueChanged += _ => fired = true;

            prop.Value = 5;

            Assert.IsFalse(fired);
        }

        [Test]
        public void SetValue_UpdatesValue()
        {
            var prop = new ReactiveProperty<int>(0);
            prop.Value = 99;
            Assert.AreEqual(99, prop.Value);
        }

        [Test]
        public void Subscribe_WithFireImmediately_InvokesCallback()
        {
            var prop = new ReactiveProperty<int>(42);
            int receivedValue = -1;
            prop.Subscribe(v => receivedValue = v, fireImmediately: true);

            Assert.AreEqual(42, receivedValue);
        }

        [Test]
        public void Subscribe_WithoutFireImmediately_DoesNotInvokeCallback()
        {
            var prop = new ReactiveProperty<int>(42);
            bool invoked = false;
            prop.Subscribe(_ => invoked = true, fireImmediately: false);

            Assert.IsFalse(invoked);
        }

        [Test]
        public void Unsubscribe_RemovesCallback()
        {
            var prop = new ReactiveProperty<int>(0);
            int callCount = 0;
            Action<int> callback = _ => callCount++;

            prop.Subscribe(callback);
            prop.Value = 1;
            Assert.AreEqual(1, callCount);

            prop.Unsubscribe(callback);
            prop.Value = 2;
            Assert.AreEqual(1, callCount, "Callback should not be called after unsubscribe");
        }

        [Test]
        public void ImplicitConversion_ReturnsValue()
        {
            var prop = new ReactiveProperty<int>(7);
            int value = prop;
            Assert.AreEqual(7, value);
        }

        [Test]
        public void ToString_ReturnsValueString()
        {
            var prop = new ReactiveProperty<int>(123);
            Assert.AreEqual("123", prop.ToString());
        }

        [Test]
        public void ToString_WithNullReference_ReturnsNullString()
        {
            var prop = new ReactiveProperty<string>();
            Assert.AreEqual("null", prop.ToString());
        }

        [Test]
        public void MultipleSubscribers_AllReceiveNotification()
        {
            var prop = new ReactiveProperty<int>(0);
            int sub1 = 0, sub2 = 0;
            prop.Subscribe(v => sub1 = v);
            prop.Subscribe(v => sub2 = v);

            prop.Value = 50;

            Assert.AreEqual(50, sub1);
            Assert.AreEqual(50, sub2);
        }
    }
}
