using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class SnapshotTests
    {
        [Fact]
        public void AddDataPoint_AddsObjectToList_WhenCalled()
        {
            var snapshot = new Snapshot(string.Empty, 0f);
            var dataPoint = new DataPoint(string.Empty, string.Empty, 0f);

            snapshot.AddDataPoint(dataPoint);

            Assert.Contains(dataPoint, snapshot.DataPoints);
        }
    }
}
