using System.Collections.Generic;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class SnapshotLoaderTests
    {
        [Fact]
        public void Load_ReturnsListOfSnapshots_WhenCalled()
        {
            var result = SnapshotLoader.Load();

            Assert.IsType<List<Snapshot>>(result);
        }
    }
}
