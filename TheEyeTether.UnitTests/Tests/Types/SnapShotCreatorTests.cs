using System.Collections.Generic;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class SnapshotCreatorTests
    {
        [Fact]
        public void Create_ReturnsListOfSnapshots_WhenPassedValidLuaTableData()
        {
            var luaTable = new Dictionary<object, object>();

            var result = SnapshotCreator.Create(luaTable);

            Assert.IsType<List<Snapshot>>(result);
        }
    }
}
