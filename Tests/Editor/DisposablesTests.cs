using NUnit.Framework;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class DisposablesTests
    {
        [Test]
        public void Empty_IsNotNull()
        {
            Assert.IsNotNull(Disposables.Empty);
        }

        [Test]
        public void Empty_Dispose_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => Disposables.Empty.Dispose());
        }

        [Test]
        public void Empty_CanBeDisposedMultipleTimes()
        {
            Assert.DoesNotThrow(() =>
            {
                Disposables.Empty.Dispose();
                Disposables.Empty.Dispose();
                Disposables.Empty.Dispose();
            });
        }
    }
}
