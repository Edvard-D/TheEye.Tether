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
            var result = SnapshotsLoader.Load();

            Assert.IsType<List<Snapshot>>(result);
        }
    }
}
