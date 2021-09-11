using System.Collections.Generic;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class SnapshotsCreatorTests
    {
        [Fact]
        public void Create_ReturnsListOfSnapshots_WhenPassedValidLuaTableAndSnapshoTypes()
        {
            var luaTable = new Dictionary<object, object>();
            var snapshotTypes = new string[1];

            var result = SnapshotsCreator.Create(luaTable, snapshotTypes);

            Assert.IsType<List<Snapshot>>(result);
        }

        [Fact]
        public void Create_ReturnsNull_WhenPassedNullLuaTable()
        {
            Dictionary<object, object> luaTable = null;
            var snapshotTypes = new string[1];

            var result = SnapshotsCreator.Create(luaTable, snapshotTypes);

            Assert.Null(result);
        }

        [Fact]
        public void Create_ReturnsNull_WhenPassedNullSnapshotTypes()
        {
            var luaTable = new Dictionary<object, object>();
            string[] snapshotTypes = null;

            var result = SnapshotsCreator.Create(luaTable, snapshotTypes);

            Assert.Null(result);
        }
    }
}
