using System.Collections.Generic;
using System.Linq;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class SnapshotsCreatorTests
    {
        [Fact]
        public void Create_ReturnsDictionaryOfListsOfSnapshots_WhenPassedValidLuaTableAndSnapshotTypes()
        {
            var categorySettingsName = "test1";
            var snapshotSettingName = "test2";
            var dataPointTypeName = "test3";
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1f } } },
                { dataPointTypeName, new Dictionary<object, object>() { { 1, 1f } } }
            };
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName }) }
            };
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingsName, new CategorySetting(categorySettingsName, snapshotSettings) }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            Assert.IsType<Dictionary<CategorySetting, Dictionary<SnapshotSetting, List<Snapshot>>>>(
                    result);
        }

        [Fact]
        public void Create_ThrowsInvalidOperationException_WhenPassedNullLuaTable()
        {
            var categorySettings = new Dictionary<string, CategorySetting>();
            var dataPointSettings = new Dictionary<string, DataPointSetting>();

            try
            {
                var result = SnapshotsCreator.Create(null, categorySettings, dataPointSettings);
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
            var categorySettingsName = "test1";
            var snapshotSettingName = "test2";
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1f } } }
            };
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, new SnapshotSetting(snapshotSettingName, new string[0]) }
            };
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingsName, new CategorySetting(categorySettingsName, snapshotSettings) }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() }
            };

            try
            {
                var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);
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
            var categorySettingsName = "test1";
            var snapshotSettingName = "test2";
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1f } } }
            };
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, new SnapshotSetting(snapshotSettingName, null) }
            };
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingsName, new CategorySetting(categorySettingsName, snapshotSettings) }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() }
            };

            try
            {
                var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);
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
        public void Create_CreatesADictionaryEntryForEachSnapshotSetting_WhenSnapshotSettingsArePassedAndHaveValidData(
                params string[] snapshotSettingNames)
        {
            var categorySettingsName = "testCategorySetting";
            var dataPointTypeName = "testDataPoint";
            var luaTable = new Dictionary<object, object>()
            {
                { dataPointTypeName, new Dictionary<object, object>() { { 1, 1f } } }
            };
            var snapshotSettings = new Dictionary<string, SnapshotSetting>();
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingsName, new CategorySetting(categorySettingsName, snapshotSettings) }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { dataPointTypeName, new DataPointSetting() }
            };
            for(int i = 0; i < snapshotSettingNames.Length; i++)
            {
                var subTable = new Dictionary<object, object>() { { 1, 1f } };
                luaTable[snapshotSettingNames[i]] = subTable;
                snapshotSettings[snapshotSettingNames[i]] = new SnapshotSetting(snapshotSettingNames[i],
                        new string[1] { dataPointTypeName });
                dataPointSettings[snapshotSettingNames[i]] = new DataPointSetting();
            }

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            Assert.Equal(snapshotSettingNames.Length, result.Keys.Count);
        }
        
        [Fact]
        public void Create_DoesNotCreateADictionaryEntryForASnapshotSetting_WhensnapshotSettingExistsButLuaTableDoesNotHaveDataForIt()
        {
            var categorySettingsName = "test1";
            var snapshotSettingName = "test2";
            var luaTable = new Dictionary<object, object>();
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, new SnapshotSetting(snapshotSettingName, new string[] { "test3" }) }
            };
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingsName, new CategorySetting(categorySettingsName, snapshotSettings) }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>();

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            Assert.Equal(0, result.Keys.Count);
        }

        [Fact]
        public void Create_DoesNotCreateADictionaryEntryForASnapshotSetting_WhenNoMatchingDataForDataPointsExists()
        {
            var categorySettingsName = "test1";
            var snapshotSettingName = "test2";
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1f } } }
            };
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, new SnapshotSetting(snapshotSettingName, new string[] { "test3" }) }
            };
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingsName, new CategorySetting(categorySettingsName, snapshotSettings) }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            Assert.Equal(0, result.Keys.Count);
        }

        [Fact]
        public void Create_UsesInputCategorySettingAsOutputDictionaryKey_WhenPassedValidLuaTable()
        {
            var categorySettingsName = "test1";
            var snapshotSettingName = "test2";
            var dataPointTypeName = "test3";
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1f } } },
                { dataPointTypeName, new Dictionary<object, object>() { { 1, 1f } } }
            };
            var snapshotSetting =  new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingsName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingsName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            Assert.Contains(categorySetting, result.Keys);
        }

        [Fact]
        public void Create_UsesInputSnapshotSettingAsOutputSubDictionaryKey_WhenPassedValidLuaTable()
        {
            var categorySettingsName = "test1";
            var snapshotSettingName = "test2";
            var dataPointTypeName = "test3";
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1f } } },
                { dataPointTypeName, new Dictionary<object, object>() { { 1, 1f } } }
            };
            var snapshotSetting =  new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingsName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingsName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            Assert.Contains(snapshotSetting, result[categorySetting].Keys);
        }

        [Theory]
        [InlineData(1f)]
        [InlineData(1f, 2f)]
        [InlineData(1f, 2f, 3f, 4f, 5f)]
        public void Create_ReturnsASnapshotForEachEntryOfASnapshotSetting_WhenPassedALuaTableWithEntriesForAsnapshotSetting(
                params object[] timestamps)
        {
            var categorySettingsName = "test1";
            var snapshotSettingName = "test2";
            var dataPointTypeName = "test3";
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
            var snapshotSetting = new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingsName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingsName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            var snapshots = result[categorySetting][snapshotSetting];
            Assert.Equal(timestamps.Length, snapshots.Count);
        }

        [Theory]
        [InlineData("test1")]
        [InlineData("test1", "test2")]
        [InlineData("test1", "test2", "test3", "test4", "test5")]
        public void Create_ReturnsASnapshotForEachEntryOfASnapshotSetting_WhenSnapshotEntryDataIsNested(
                params string[] subTableNames)
        {
            var categorySettingsName = "test1";
            var snapshotSettingName = "test2";
            var dataPointTypeName = "test3";
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
            var snapshotSetting = new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingsName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingsName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            var snapshots = result[categorySetting][snapshotSetting];
            Assert.Equal(subTableNames.Length * entriesPerSubTable, snapshots.Count);
        }

        [Fact]
        public void Create_AddsEntriesOfDataPointTypesToTheSnapshot_WhenEntriesForADataPointTypeAreNotNested()
        {
            var categorySettingsName = "test1";
            var snapshotSettingName = "test2";
            var dataPointTypeName = "test3";
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1f } } },
                { dataPointTypeName, new Dictionary<object, object>() { { 1, 1f } } }
            };
            var snapshotSetting = new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingsName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingsName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            var matchingDataPoints = result[categorySetting][snapshotSetting][0].DataPoints
                    .Where(dp => dp.TypeName == dataPointTypeName)
                    .ToList();
            Assert.Equal(1, matchingDataPoints.Count);
        }

        [Fact]
        public void Create_AddsEntriesOfDataPointTypesToTheSnapshot_WhenEntriesForADataPointTypeAreNested()
        {
            var categorySettingsName = "test1";
            var snapshotSettingName = "test2";
            var dataPointTypeName = "test3";
            var subTable = new Dictionary<object, object>() { { 1, 1f } };
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1f } } },
                { dataPointTypeName, new Dictionary<object, object>() { { "test", subTable } } }
            };
            var snapshotSetting = new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingsName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingsName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            var matchingDataPoints = result[categorySetting][snapshotSetting][0].DataPoints
                    .Where(dp => dp.TypeName == dataPointTypeName)
                    .ToList();
            Assert.Equal(1, matchingDataPoints.Count);
        }

        [Fact]
        public void Create_DoesNotAddEntryOfDataPointType_WhenDataPointTypeIsInSnapshotSettingButNoDataForItExists()
        {
            var categorySettingsName = "test1";
            var snapshotSettingName = "test2";
            var dataPointTypeName = "test3";
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1f } } },
            };
            var snapshotSetting = new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingsName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingsName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            Assert.DoesNotContain(categorySetting, result.Keys);
        }

        [Theory]
        [InlineData(1f, 2f, 3f)]
        [InlineData(1f, 1.5f, 3f)]
        [InlineData(1f)]
        public void Create_AddsDataPointWithRangeThatSnapshotTimestampFallsBetween_WhenADataPointTimestampWithAStartTimeLessThanTheSnapshotTimestampExists(
                params float[] dataPointTimestamps)
        {
            var categorySettingsName = "test1";
            var snapshotSettingName = "test2";
            var dataPointTypeName = "test3";
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
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingsName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingsName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            var correctTimeStamp = dataPointTimestamps[0];
            for(int i = 1; i < dataPointTimestamps.Length - 1; i++)
            {
                var timestamp = dataPointTimestamps[i];
                var nextTimestamp = dataPointTimestamps[i + 1];

                if(timestamp > correctTimeStamp
                        && timestamp <= snapshotTimeStamp
                        && nextTimestamp > snapshotTimeStamp)
                {
                    correctTimeStamp = timestamp;
                }
            }
            var matchingDataPoint = result[categorySetting][snapshotSetting][0].DataPoints
                    .Where(dp => dp.TypeName == dataPointTypeName)
                    .First();
            Assert.Equal(correctTimeStamp, matchingDataPoint.TimestampRange.Start);
        }

        [Fact]
        public void Create_AddsMultipleDataPoints_WhenMoreThanOneForADataTypeHasARangeThatTheSnapshotTimestampFallsBetween()
        {
            var categorySettingsName = "test1";
            var snapshotSettingName = "test2";
            var dataPointTypeName = "test3";
            var trueSubTable = new Dictionary<object, object>() { { 1, 1f } };
            var falseSubTable = new Dictionary<object, object>() { { 1, 3f } };
            var snapshotTimeStamp = 2f;
            var luaTable = new Dictionary<object, object>()
            {
                { snapshotSettingName, new Dictionary<object, object>() { { 1, snapshotTimeStamp } } },
                {
                    dataPointTypeName, new Dictionary<object, object>()
                    {
                        { "test1_true", trueSubTable },
                        { "test1_false", falseSubTable },
                        { "test2_true", trueSubTable },
                        { "test2_false", falseSubTable }
                    }
                }
            };
            var snapshotSetting = new SnapshotSetting(snapshotSettingName,
                    new string[] { dataPointTypeName });
            var snapshotSettings = new Dictionary<string, SnapshotSetting>()
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingsName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingsName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting("false", 1) }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            Assert.Equal(2, result[categorySetting][snapshotSetting][0].DataPoints.Count);
        }

        [Theory]
        [InlineData(3f)]
        [InlineData(3f, 4.5f, 5f)]
        public void Create_DoesNotCreateASnapshot_WhenAllDataPointTimestampsAreLessThanEqualToSnapshotTimestamp(
                params float[] dataPointTimestamps)
        {
            var categorySettingsName = "test1";
            var snapshotSettingName = "test2";
            var dataPointTypeName = "test3";
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
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingsName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingsName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            Assert.DoesNotContain(categorySetting, result.Keys);
        }
    }
}
