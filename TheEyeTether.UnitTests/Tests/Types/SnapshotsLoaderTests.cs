using System.Collections.Generic;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class SnapshotsLoaderTests
    {
        [Fact]
        public void Load_ReturnsListOfSnapshots_WhenCalled()
        {
            var directoryPath = string.Empty;

            var result = SnapshotsLoader.Load(directoryPath, 1);

            Assert.IsType<List<Snapshot>>(result);
        }

        [Fact]
        public void Load_ThrowsInvalidOperationException_WhenPassedNullDirectoryPath()
        {
            try
            {
                var result = SnapshotsLoader.Load(null, 1);
                Assert.True(false);
            }
            catch(System.Exception ex)
            {
                Assert.IsType<System.InvalidOperationException>(ex);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Load_ThrowsInvalidOperationException_WhenPassedNegativeOrZeroLookbackDays(int lookbackDays)
        {
            try
            {
                var result = SnapshotsLoader.Load(null, lookbackDays);
                Assert.True(false);
            }
            catch(System.Exception ex)
            {
                Assert.IsType<System.InvalidOperationException>(ex);
            }
        }
    }
}
