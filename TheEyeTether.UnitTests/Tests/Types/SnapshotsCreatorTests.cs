using System.Collections.Generic;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class SnapshotsCreatorTests
    {
        [Fact]
        public void Create_ReturnsDictionaryOfListsOfSnapshots_WhenPassedValidLuaTableAndSnapshoTypes()
        {
            var luaTable = new Dictionary<object, object>();
            var snapshotTypes = new SnapshotType[1];

            var result = SnapshotsCreator.Create(luaTable, snapshotTypes);

            Assert.IsType<Dictionary<SnapshotType, List<Snapshot>>>(result);
        }

        [Fact]
        public void Create_ReturnsNull_WhenPassedNullLuaTable()
        {
            Dictionary<object, object> luaTable = null;
            var snapshotTypes = new SnapshotType[1];

            var result = SnapshotsCreator.Create(luaTable, snapshotTypes);

            Assert.Null(result);
        }

        [Fact]
        public void Create_ReturnsNull_WhenPassedNullSnapshotTypes()
        {
            var luaTable = new Dictionary<object, object>();
            SnapshotType[] snapshotTypes = null;

            var result = SnapshotsCreator.Create(luaTable, snapshotTypes);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("test")]
        [InlineData("test1, test2")]
        [InlineData("test1, test2, test3, test4, test5")]
        public void Create_ReturnsADictionaryEntryForEachSnapshotType_WhenSnapshotTypesArePassed(
                params string[] snapshotTypeNames)
        {
            var luaTable = new Dictionary<object, object>();
            var snapshotTypes = new SnapshotType[snapshotTypeNames.Length];
            for(int i = 0; i < snapshotTypeNames.Length; i++)
            {
                snapshotTypes[i] = new SnapshotType(snapshotTypeNames[i]);
            }

            var result = SnapshotsCreator.Create(luaTable, snapshotTypes);

            Assert.Equal(snapshotTypes.Length, result.Keys.Count);
        }

        [Theory]
        [InlineData(1f)]
        [InlineData(1f, 2f)]
        [InlineData(1f, 2f, 3f, 4f, 5f)]
        public void Create_ReturnsASnapshotForEachEntryOfASnapshotType_WhenPassedALuaTableWithEntriesForASnapshotType(
                params float[] timestamps)
        {
            var snapshotTypeName = "test";
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotTypeName, timestamps }
            };
            var snapshotTypes = new SnapshotType[] { new SnapshotType(snapshotTypeName) };

            var result = SnapshotsCreator.Create(luaTable, snapshotTypes);

            var snapshots = result[snapshotTypes[0]] as List<Snapshot>;
            Assert.Equal(timestamps.Length, snapshots.Count);
        }
    }
}
