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

            var result = SnapshotsLoader.Load(directoryPath);

            Assert.IsType<List<Snapshot>>(result);
        }

        [Fact]
        public void Load_ThrowsInvalidOperationException_WhenPassedNullDirectoryPath()
        {
            try
            {
                var result = SnapshotsLoader.Load(null);
                Assert.True(false);
            }
            catch(System.Exception ex)
            {
                Assert.IsType<System.InvalidOperationException>(ex);
            }
        }
    }
}
