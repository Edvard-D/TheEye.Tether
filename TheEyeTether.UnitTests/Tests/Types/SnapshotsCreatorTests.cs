using System.Collections.Generic;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class SnapshotsCreatorTests
    {
        [Fact]
        public void Create_ReturnsListOfSnapshots_WhenPassedValidLuaTableData()
        {
            var luaTable = new Dictionary<object, object>();

            var result = SnapshotsCreator.Create(luaTable);

            Assert.IsType<List<Snapshot>>(result);
        }

        [Fact]
        public void Create_ReturnsNull_WhenPassedNullLuaTable()
        {
            Dictionary<object, object> luaTable = null;

            var result = SnapshotsCreator.Create(luaTable);

            Assert.Null(result);
        }
    }
}
