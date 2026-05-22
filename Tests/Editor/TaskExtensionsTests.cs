using NUnit.Framework;
using System.Threading.Tasks;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class TaskExtensionsTests
    {
        [Test]
        public void IsRunning_WithNullTask_ReturnsFalse()
        {
            Task nullTask = null;
            Assert.IsFalse(nullTask.IsRunning());
        }

        [Test]
        public void IsRunning_WithCompletedTask_ReturnsFalse()
        {
            var task = Task.CompletedTask;
            Assert.IsFalse(task.IsRunning());
        }

        [Test]
        public async Task IsRunning_WithRunningTask_ReturnsTrue()
        {
            var task = Task.Delay(10000);
            Assert.IsTrue(task.IsRunning());
            // Don't await the delay, just cancel
        }
    }
}
