using System.Collections.Generic;
using System.Linq;
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

        [Fact]
        public void Create_UsesInputTableKeysAsOutputDictionaryKeys_WhenPassedValidLuaTable()
        {
            var tableName = "test";
            var luaTable = new Dictionary<object, object>()
            {
                { tableName, new Dictionary<object, object>() { { 1, 1f } } }
            };

            var result = DataPointsCreator.Create(luaTable);

            Assert.Contains(tableName, result.Keys);
        }

        [Theory]
        [InlineData(1f)]
        [InlineData(1f, 2f)]
        [InlineData(1f, 2f, 3f, 4f, 5f)]
        public void Create_ReturnsADataPointForEachDataEntry_WhenEntriesAreNotNested(
                params float[] timestamps)
        {
            var subTable = new Dictionary<object, object>();
            for(int i = 0; i < timestamps.Length; i++)
            {
                subTable[i + 1] = timestamps[i];
            }
            var tableName = "test";
            var luatable = new Dictionary<object, object>()
            {
                { tableName, subTable }
            };

            var result = DataPointsCreator.Create(luatable);

            Assert.Equal(timestamps.Length, result[tableName].Count);
        }

        [Theory]
        [InlineData(1f)]
        [InlineData(1f, 2f)]
        [InlineData(1f, 2f, 3f, 4f, 5f)]
        public void Create_ReturnsADataPointForEachDataEntry_WhenEntriesAreNested(
                params float[] timestamps)
        {
            var subTable = new Dictionary<object, object>();
            for(int i = 0; i < timestamps.Length; i++)
            {
                subTable[i + 1] = timestamps[i];
            }
            var tableName = "test1";
            var subTableName = "test2";
            var luatable = new Dictionary<object, object>()
            {
                { tableName, new Dictionary<object, object>() { { subTableName, subTable } } }
            };

            var result = DataPointsCreator.Create(luatable);

            Assert.Equal(timestamps.Length, result[tableName].Count);
        }

        [Fact]
        public void Create_AssignsTopLevelTableKeyAsDataPointTypeName_WhenPassedValidLuaTable()
        {
            var tableName = "test";
            var luaTable = new Dictionary<object, object>()
            {
                { tableName, new Dictionary<object, object>() { { 1, 1f } } }
            };

            var result = DataPointsCreator.Create(luaTable);

            Assert.Equal(tableName, result[tableName][0].TypeName);
        }

        [Fact]
        public void Create_AssignsTopLevelTableKeyAsDataPointSubTypeName_WhenEntiresAreNotNested()
        {
            var tableName = "test";
            var luaTable = new Dictionary<object, object>()
            {
                { tableName, new Dictionary<object, object>() { { 1, 1f } } }
            };

            var result = DataPointsCreator.Create(luaTable);

            Assert.Equal(tableName, result[tableName][0].SubTypeName);
        }

        [Fact]
        public void Create_AssignsSubTableKeyAsDataPointSubTypeName_WhenEntriesAreNested()
        {
            var tableName = "test1";
            var subTableName = "test2";
            var subTable = new Dictionary<object, object>() { { 1, 1f } };
            var luaTable = new Dictionary<object, object>()
            {
                { tableName, new Dictionary<object, object>() { { subTableName, subTable } } }
            };

            var result = DataPointsCreator.Create(luaTable);

            Assert.Equal(subTableName, result[tableName][0].SubTypeName);
        }

        [Fact]
        public void Create_AssignsDataEntryValueAsDataPointTimestampRangeStart_WhenPassedValidLuaTable()
        {
            var tableName = "test";
            var timestamp = 1f;
            var luaTable = new Dictionary<object, object>()
            {
                { tableName, new Dictionary<object, object>() { { 1, timestamp } } }
            };

            var result = DataPointsCreator.Create(luaTable);

            Assert.Equal(timestamp, result[tableName][0].TimestampRange.Start);
        }

        [Theory]
        [InlineData(1f, 2f)]
        [InlineData(2f, 1f)]
        [InlineData(1f, 2f, 5f, 4f, 3f)]
        public void Create_AssignsNextLargestValueAsDataPointTimestampRangeEnd_WhenThereIsALargerTimestamp(
                params float[] timestamps)
        {
            var tableName = "test";
            var subTable = new Dictionary<object, object>();
            for(int i = 0; i < timestamps.Length; i++)
            {
                subTable[i + 1] = timestamps[i];
            }
            var luaTable = new Dictionary<object, object>()
            {
                { tableName, subTable }
            };

            var result = DataPointsCreator.Create(luaTable);

            var sortedTimestamps = timestamps.ToList();
            sortedTimestamps.Sort();
            for(int i = 0; i < sortedTimestamps.Count - 1; i++)
            {
                var dataPoint = result[tableName]
                        .Where(dp => dp.TimestampRange.Start == sortedTimestamps[i])
                        .First();
                Assert.Equal(sortedTimestamps[i + 1], dataPoint.TimestampRange.End);
            }
        }

        [Theory]
        [InlineData(1f)]
        [InlineData(1f, 2f)]
        [InlineData(2f, 1f)]
        [InlineData(1f, 2f, 5f, 4f, 3f)]
        public void Create_AssignsMaxValueAsDataPointTimestampRangeEnd_WhenLargestTimestamp(
                params float[] timestamps)
        {
            var tableName = "test";
            var subTable = new Dictionary<object, object>();
            for(int i = 0; i < timestamps.Length; i++)
            {
                subTable[i + 1] = timestamps[i];
            }
            var luaTable = new Dictionary<object, object>()
            {
                { tableName, subTable }
            };

            var result = DataPointsCreator.Create(luaTable);

            var largestTimestamp = timestamps.ToList().Max();
            var largestTimestampDataPoint = result[tableName]
                    .Where(dp => dp.TimestampRange.Start == largestTimestamp)
                    .First();
            Assert.Equal(float.MaxValue, largestTimestampDataPoint.TimestampRange.End);
        }
    }
}
