using System.Collections.Generic;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class DataPointsCreatorTests
    {
        [Fact]
        public void Create_ReturnsDictionaryOfListsOfDataPoints_WhenPassedValidLuaTable()
        {
            var luaTable = new Dictionary<object, object>()
            {
                { "test", new Dictionary<object, object>() { { 1, 1f } } }
            };

            var result = DataPointsCreator.Create(luaTable);

            Assert.IsType<Dictionary<string, List<DataPoint>>>(result);
        }

        [Fact]
        public void Create_ReturnsNull_WhenPassedNullLuaTable()
        {
            var result = DataPointsCreator.Create(null);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("test1")]
        [InlineData("test1", "test2")]
        [InlineData("test1", "test2", "test3", "test4", "test5")]
        public void Create_CreatesADictionaryEntryForEachFirstLevelSubTable_WhenPassedValidLuaTable(
                params string[] tableNames)
        {
            var luaTable = new Dictionary<object, object>();
            foreach(string tableName in tableNames)
            {
                luaTable[tableName] = new Dictionary<object, object>() { { 1, 1f } };
            }

            var result = DataPointsCreator.Create(luaTable);

            Assert.Equal(tableNames.Length, result.Keys.Count);
        }
    }
}
