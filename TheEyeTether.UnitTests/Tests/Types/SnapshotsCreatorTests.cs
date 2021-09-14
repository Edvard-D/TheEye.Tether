using System.Collections.Generic;
using System.Linq;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class SnapshotsCreatorTests
    {
        [Fact]
        public void Create_ReturnsDictionaryOfListsOfSnapshots_WhenPassedValidLuaTableAndSnapshoTypes()
        {
            var snapshotSettingName = "test1";
            var dataPointTypeName = "test2";
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1f } } },
                { dataPointTypeName, new Dictionary<object, object>() { { 1, 1f } } }
            };
            var snapshotSettings = new SnapshotSetting[1] { new SnapshotSetting(snapshotSettingName,
                    new string[] { dataPointTypeName })};
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, snapshotSettings, dataPointSettings);

            Assert.IsType<Dictionary<SnapshotSetting, List<Snapshot>>>(result);
        }

        [Fact]
        public void Create_ThrowsInvalidOperationException_WhenPassedNullLuaTable()
        {
            var snapshotSettings = new SnapshotSetting[1];
            var dataPointSettings = new Dictionary<string, DataPointSetting>();

            try
            {
                var result = SnapshotsCreator.Create(null, snapshotSettings, dataPointSettings);
                Assert.True(false);
            }
            catch(System.Exception ex)
            {
                Assert.IsType<System.InvalidOperationException>(ex);
            }
        }

        [Fact]
        public void Create_ThrowsInvalidOperationException_WhenPassedNullSnapshotSettings()
        {
            var luaTable = new Dictionary<object, object>();
            var dataPointSettings = new Dictionary<string, DataPointSetting>();

            try
            {
                var result = SnapshotsCreator.Create(luaTable, null, dataPointSettings);
                Assert.True(false);
            }
            catch(System.Exception ex)
            {
                Assert.IsType<System.InvalidOperationException>(ex);
            }
        }
        
        [Fact]
        public void Create_ThrowsInvalidOperationException_WhenSnapshotSettingHasNoDataPointTypeNames()
        {
            var snapshotSettingName = "test1";
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1f } } }
            };
            var snapshotSettings = new SnapshotSetting[] { new SnapshotSetting(snapshotSettingName,
                    new string[0]) };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() }
            };

            try
            {
                var result = SnapshotsCreator.Create(luaTable, snapshotSettings, dataPointSettings);
                Assert.True(false);
            }
            catch(System.Exception ex)
            {
                Assert.IsType<System.InvalidOperationException>(ex);
            }
        }
        
        [Fact]
        public void Create_ThrowsInvalidOperationException_WhenSnapshotSettingDataPointTypeNamesIsNull()
        {
            var snapshotSettingName = "test1";
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1f } } }
            };
            var snapshotSettings = new SnapshotSetting[] { new SnapshotSetting(snapshotSettingName, null) };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() }
            };

            try
            {
                var result = SnapshotsCreator.Create(luaTable, snapshotSettings, dataPointSettings);
                Assert.True(false);
            }
            catch(System.Exception ex)
            {
                Assert.IsType<System.InvalidOperationException>(ex);
            }
        }

        [Theory]
        [InlineData("test")]
        [InlineData("test1, test2")]
        [InlineData("test1, test2, test3, test4, test5")]
        public void Create_CreatesADictionaryEntryForEachsnapshotSetting_WhensnapshotSettingsArePassedAndHaveValidData(
                params string[] snapshotSettingNames)
        {
            var dataPointTypeName = "testDataPoint";
            var luaTable = new Dictionary<object, object>()
            {
                { dataPointTypeName, new Dictionary<object, object>() { { 1, 1f } } }
            };
            var snapshotSettings = new SnapshotSetting[snapshotSettingNames.Length];
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { dataPointTypeName, new DataPointSetting() }
            };
            for(int i = 0; i < snapshotSettingNames.Length; i++)
            {
                var subTable = new Dictionary<object, object>() { { 1, 1f } };
                luaTable[snapshotSettingNames[i]] = subTable;
                snapshotSettings[i] = new SnapshotSetting(snapshotSettingNames[i],
                        new string[1] { dataPointTypeName });
                dataPointSettings[snapshotSettingNames[i]] = new DataPointSetting();
            }

            var result = SnapshotsCreator.Create(luaTable, snapshotSettings, dataPointSettings);

            Assert.Equal(snapshotSettings.Length, result.Keys.Count);
        }
        
        [Fact]
        public void Create_DoesNotCreateADictionaryEntryForAsnapshotSetting_WhensnapshotSettingExistsButLuaTableDoesNotHaveDataForIt()
        {
            var snapshotSettingName = "test1";
            var luaTable = new Dictionary<object, object>();
            var snapshotSettings = new SnapshotSetting[] { new SnapshotSetting(snapshotSettingName,
                    new string[] { "test2" }) };
            var dataPointSettings = new Dictionary<string, DataPointSetting>();

            var result = SnapshotsCreator.Create(luaTable, snapshotSettings, dataPointSettings);

            Assert.Equal(0, result.Keys.Count);
        }

        [Fact]
        public void Create_DoesNotCreateADictionaryEntryForAsnapshotSetting_WhenNoMatchingDataForDataPointsExists()
        {
            var snapshotSettingName = "test1";
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1f } } }
            };
            var snapshotSettings = new SnapshotSetting[] { new SnapshotSetting(snapshotSettingName,
                    new string[] { "test2" }) };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, snapshotSettings, dataPointSettings);

            Assert.Equal(0, result.Keys.Count);
        }

        [Fact]
        public void Create_UsesInputsnapshotSettingAsOutputDictionaryKey_WhenPassedValidLuaTable()
        {
            var snapshotSettingName = "test1";
            var dataPointTypeName = "test2";
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1f } } },
                { dataPointTypeName, new Dictionary<object, object>() { { 1, 1f } } }
            };
            var snapshotSetting =  new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new SnapshotSetting[] { snapshotSetting };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, snapshotSettings, dataPointSettings);

            Assert.Contains(snapshotSetting, result.Keys);
        }

        [Theory]
        [InlineData(1f)]
        [InlineData(1f, 2f)]
        [InlineData(1f, 2f, 3f, 4f, 5f)]
        public void Create_ReturnsASnapshotForEachEntryOfAsnapshotSetting_WhenPassedALuaTableWithEntriesForAsnapshotSetting(
                params object[] timestamps)
        {
            var snapshotSettingName = "test1";
            var dataPointTypeName = "test2";
            var subTableTimestamps = new Dictionary<object, object>();
            for(int i = 0; i < timestamps.Length; i++)
            {
                subTableTimestamps[i + 1] = (float)timestamps[i];
            }
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, subTableTimestamps },
                { dataPointTypeName, new Dictionary<object, object>() { { 1, 1f } } }
            };
            var snapshotSettings = new SnapshotSetting[] { new SnapshotSetting(snapshotSettingName,
                    new string[] { dataPointTypeName }) };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, snapshotSettings, dataPointSettings);

            var snapshots = result[snapshotSettings[0]] as List<Snapshot>;
            Assert.Equal(timestamps.Length, snapshots.Count);
        }

        [Theory]
        [InlineData("test1")]
        [InlineData("test1", "test2")]
        [InlineData("test1", "test2", "test3", "test4", "test5")]
        public void Create_ReturnsASnapshotForEachEntryOfAsnapshotSetting_WhenSnapshotEntryDataIsNested(
                params string[] subTableNames)
        {
            var snapshotSettingName = "test";
            var dataPointTypeName = "test2";
            var entriesPerSubTable = 5;
            var subTables = new Dictionary<object, object>();
            for(int i = 0; i < subTableNames.Length; i++)
            {
                var subTableTimestamps = new Dictionary<object, object>();
                for(int j = 0; j < entriesPerSubTable; j++)
                {
                    subTableTimestamps[j + 1] = (float)(i + 1) + (j + 1);
                }

                subTables[subTableNames[i]] = subTableTimestamps;
            }
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, subTables },
                { dataPointTypeName, new Dictionary<object, object>() { { 1, 1f } } }
            };
            var snapshotSettings = new SnapshotSetting[] { new SnapshotSetting(snapshotSettingName,
                    new string[] { dataPointTypeName }) };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, snapshotSettings, dataPointSettings);

            var snapshots = result[snapshotSettings[0]] as List<Snapshot>;
            Assert.Equal(subTableNames.Length * entriesPerSubTable, snapshots.Count);
        }

        [Fact]
        public void Create_AddsEntriesOfDataPointTypesToTheSnapshot_WhenEntriesForADataPointTypeAreNotNested()
        {
            var snapshotSettingName = "test1";
            var dataPointTypeName = "test2";
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1f } } },
                { dataPointTypeName, new Dictionary<object, object>() { { 1, 1f } } }
            };
            var snapshotSettings = new SnapshotSetting[]
            {
                new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName })
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, snapshotSettings, dataPointSettings);

            var matchingDataPoints = result[snapshotSettings[0]][0].DataPoints
                    .Where(dp => dp.TypeName == dataPointTypeName)
                    .ToList();
            Assert.Equal(1, matchingDataPoints.Count);
        }

        [Fact]
        public void Create_AddsEntriesOfDataPointTypesToTheSnapshot_WhenEntriesForADataPointTypeAreNested()
        {
            var snapshotSettingName = "test1";
            var dataPointTypeName = "test2";
            var subTable = new Dictionary<object, object>() { { 1, 1f } };
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1f } } },
                { dataPointTypeName, new Dictionary<object, object>() { { "test", subTable } } }
            };
            var snapshotSettings = new SnapshotSetting[]
            {
                new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName })
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, snapshotSettings, dataPointSettings);

            var matchingDataPoints = result[snapshotSettings[0]][0].DataPoints
                    .Where(dp => dp.TypeName == dataPointTypeName)
                    .ToList();
            Assert.Equal(1, matchingDataPoints.Count);
        }

        [Fact]
        public void Create_DoesNotAddEntryOfDataPointType_WhenDataPointTypeIsInSnapshotSettingButNoDataForItExists()
        {
            var snapshotSettingName = "test1";
            var dataPointTypeName = "test2";
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1f } } },
            };
            var snapshotSetting = new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new SnapshotSetting[] { snapshotSetting };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, snapshotSettings, dataPointSettings);

            Assert.DoesNotContain(snapshotSetting, result.Keys);
        }

        [Theory]
        [InlineData(1f, 2f, 3f)]
        [InlineData(1f, 1.5f, 3f)]
        [InlineData(1f)]
        public void Create_AddsDataPointWithHighestTimestampThatIsLessThanEqualToSnapshotTimestamp_WhenDataPointTimestampsAreAllLessThanEqualToSnapshotTimestamp(
                params float[] dataPointTimestamps)
        {
            var snapshotSettingName = "test1";
            var dataPointTypeName = "test2";
            var snapshotTimeStamp = 2f;
            var subTable = new Dictionary<object, object>();
            for(int i = 0; i < dataPointTimestamps.Length; i++)
            {
                subTable[i + 1] = dataPointTimestamps[i];
            }
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, snapshotTimeStamp } } },
                { dataPointTypeName, new Dictionary<object, object>() { { "test", subTable } } }
            };
            var snapshotSettings = new SnapshotSetting[]
            {
                new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName })
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, snapshotSettings, dataPointSettings);

            var correctTimeStamp = dataPointTimestamps.ToList()
                    .Where(dpt => dpt <= snapshotTimeStamp)
                    .Max();
            var matchingDataPoint = result[snapshotSettings[0]][0].DataPoints
                    .Where(dp => dp.TypeName == dataPointTypeName)
                    .First();
            Assert.Equal(correctTimeStamp, matchingDataPoint.TimestampRange.Start);
        }

        [Theory]
        [InlineData(3f)]
        [InlineData(3f, 4.5f, 5f)]
        public void Create_DoesNotCreateASnapshot_WhenAllDataPointTimestampsAreLessThanEqualToSnapshotTimestamp(
                params float[] dataPointTimestamps)
        {
            var snapshotSettingName = "test1";
            var dataPointTypeName = "test2";
            var snapshotTimeStamp = 2f;
            var subTable = new Dictionary<object, object>();
            for(int i = 0; i < dataPointTimestamps.Length; i++)
            {
                subTable[i + 1] = dataPointTimestamps[i];
            }
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, snapshotTimeStamp } } },
                { dataPointTypeName, new Dictionary<object, object>() { { "test", subTable } } }
            };
            var snapshotSetting = new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new SnapshotSetting[] { snapshotSetting };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, snapshotSettings, dataPointSettings);

            Assert.DoesNotContain(snapshotSetting, result.Keys);
        }
    }
}
