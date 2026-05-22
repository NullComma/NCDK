using NUnit.Framework;
using System;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class ServiceLocatorTests
    {
        public interface ITestService
        {
            string GetName();
        }

        public class TestService : ITestService, IDisposable
        {
            public string GetName() => "TestService";
            public bool IsDisposed { get; private set; }
            public void Dispose() => IsDisposed = true;
        }

        public class AnotherTestService : ITestService
        {
            public string GetName() => "AnotherTestService";
        }

        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Register<ITestService>(new TestService());
        }

        [TearDown]
        public void TearDown()
        {
            var field = typeof(ServiceLocator).GetField("_instances",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var instances = field.GetValue(null) as System.Collections.Concurrent.ConcurrentDictionary<Type, object>;
            instances?.Clear();

            var lazyField = typeof(ServiceLocator).GetField("_lazyFactories",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var lazyInstances = lazyField.GetValue(null) as System.Collections.Concurrent.ConcurrentDictionary<Type, Func<object>>;
            lazyInstances?.Clear();
        }

        [Test]
        public void Resolve_WithRegisteredInstance_ReturnsInstance()
        {
            var service = ServiceLocator.Resolve<ITestService>();
            Assert.IsNotNull(service);
            Assert.AreEqual("TestService", service.GetName());
        }

        [Test]
        public void Resolve_WithRegisteredInstance_ReturnsSameInstance()
        {
            var service1 = ServiceLocator.Resolve<ITestService>();
            var service2 = ServiceLocator.Resolve<ITestService>();
            Assert.AreSame(service1, service2);
        }

        [Test]
        public void Resolve_WithUnregisteredType_ThrowsException()
        {
            Assert.Throws<Exception>(() => ServiceLocator.Resolve<string>());
        }

        [Test]
        public void Register_OverwritesExistingInstance()
        {
            ServiceLocator.Register<ITestService>(new AnotherTestService());
            var service = ServiceLocator.Resolve<ITestService>();
            Assert.AreEqual("AnotherTestService", service.GetName());
        }

        [Test]
        public void Register_WithActivator_CreatesInstance()
        {
            ServiceLocator.Register<TestService>();
            var service = ServiceLocator.Resolve<TestService>();
            Assert.IsNotNull(service);
            Assert.IsInstanceOf<TestService>(service);
        }

        [Test]
        public void RegisterLazy_CreatesInstanceOnFirstResolve()
        {
            // Clear any existing registration first
            var field = typeof(ServiceLocator).GetField("_instances",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var instances = field.GetValue(null) as System.Collections.Concurrent.ConcurrentDictionary<Type, object>;
            instances?.Clear();

            int creationCount = 0;
            ServiceLocator.RegisterLazy<ITestService>(() =>
            {
                creationCount++;
                return new TestService();
            });

            var service = ServiceLocator.Resolve<ITestService>();
            Assert.IsNotNull(service);
            Assert.AreEqual(1, creationCount);

            ServiceLocator.Resolve<ITestService>();
            Assert.AreEqual(1, creationCount, "Factory should only be called once");
        }

        [Test]
        public void Get_IsAliasForResolve()
        {
            var service = ServiceLocator.Get<ITestService>();
            Assert.IsNotNull(service);
            Assert.AreEqual("TestService", service.GetName());
        }

        [Test]
        public void Resolve_OutParameter_SetsInstance()
        {
            ServiceLocator.Resolve<ITestService>(out var service);
            Assert.IsNotNull(service);
            Assert.AreEqual("TestService", service.GetName());
        }

        [Test]
        public void Resolve_ByType_ReturnsInstance()
        {
            ServiceLocator.Register<ITestService>(new TestService());
            var service = ServiceLocator.Resolve(typeof(ITestService));
            Assert.IsNotNull(service);
            Assert.IsInstanceOf<ITestService>(service);
        }

        [Test]
        public void Resolve_ByType_WithLazy_CreatesInstance()
        {
            ServiceLocator.RegisterLazy<ITestService>(() => new TestService());
            var service = ServiceLocator.Resolve(typeof(ITestService));
            Assert.IsNotNull(service);
            Assert.IsInstanceOf<ITestService>(service);
        }

        [Test]
        public void Register_WithGenericObjectType_ThrowsException()
        {
            Assert.Throws<Exception>(() => ServiceLocator.Register<UnityEngine.Object>(new UnityEngine.Object()));
        }

        [Test]
        public void RegisterLazy_WithGenericObjectType_ThrowsException()
        {
            Assert.Throws<Exception>(() => ServiceLocator.RegisterLazy<UnityEngine.Object>(() => new UnityEngine.Object()));
        }
    }
}
