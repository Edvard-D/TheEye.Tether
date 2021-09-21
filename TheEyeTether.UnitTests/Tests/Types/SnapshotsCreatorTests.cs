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
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1d } } },
                { dataPointTypeName, new Dictionary<object, object>() { { 1, 1d } } }
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

            Assert.IsType<Dictionary<Category, Dictionary<SnapshotSetting, List<Snapshot>>>>(
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
            var categorySettingName = "test1";
            var snapshotSettingName = "test2";
            var luaTable = new Dictionary<object, object>()
            {
                { categorySettingName, new Dictionary<object, object>()
                    {
                        { "test3", new Dictionary<object, object>() { { 1, 1d } } }
                    }
                },
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1d } } }
            };
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, new SnapshotSetting(snapshotSettingName, new string[0]) }
            };
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingName, new CategorySetting(categorySettingName, snapshotSettings) }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { categorySettingName, new DataPointSetting() },
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
            var categorySettingName = "test1";
            var snapshotSettingName = "test2";
            var luaTable = new Dictionary<object, object>()
            {
                { categorySettingName, new Dictionary<object, object>()
                    {
                        { "test3", new Dictionary<object, object>() { { 1, 1d } } }
                    }
                },
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1d } } }
            };
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, new SnapshotSetting(snapshotSettingName, null) }
            };
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingName, new CategorySetting(categorySettingName, snapshotSettings) }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { categorySettingName, new DataPointSetting() },
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
            var categorySettingName = "testCategorySetting";
            var dataPointTypeName = "testDataPoint";
            var luaTable = new Dictionary<object, object>()
            {
                { categorySettingName, new Dictionary<object, object>()
                    {
                        { "testSubTable", new Dictionary<object, object>() { { 1, 1d } } }
                    }
                },
                { dataPointTypeName, new Dictionary<object, object>() { { 1, 1d } } }
            };
            var snapshotSettings = new Dictionary<string, SnapshotSetting>();
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingName, new CategorySetting(categorySettingName, snapshotSettings) }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { categorySettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };
            for(int i = 0; i < snapshotSettingNames.Length; i++)
            {
                var subTable = new Dictionary<object, object>() { { 1, 1d } };
                luaTable[snapshotSettingNames[i]] = subTable;
                snapshotSettings[snapshotSettingNames[i]] = new SnapshotSetting(snapshotSettingNames[i],
                        new string[1] { dataPointTypeName });
                dataPointSettings[snapshotSettingNames[i]] = new DataPointSetting();
            }

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            Assert.Equal(snapshotSettingNames.Length, result.Keys.Count);
        }
        
        [Fact]
        public void Create_DoesNotCreateADictionaryEntryForASnapshotSetting_WhenSnapshotSettingExistsButLuaTableDoesNotHaveDataForIt()
        {
            var categorySettingName = "test1";
            var snapshotSettingName = "test2";
            var luaTable = new Dictionary<object, object>()
            {
                { categorySettingName, new Dictionary<object, object>()
                    {
                        { "test3", new Dictionary<object, object>() { { 1, 1d } } }
                    }
                }
            };
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, new SnapshotSetting(snapshotSettingName, new string[] { "test4" }) }
            };
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingName, new CategorySetting(categorySettingName, snapshotSettings) }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { categorySettingName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            Assert.Equal(0, result.Keys.Count);
        }

        [Fact]
        public void Create_DoesNotCreateADictionaryEntryForASnapshotSetting_WhenNoMatchingDataForDataPointsExists()
        {
            var categorySettingName = "test1";
            var snapshotSettingName = "test2";
            var luaTable = new Dictionary<object, object>()
            {
                { categorySettingName, new Dictionary<object, object>()
                    {
                        { "test3", new Dictionary<object, object>() { { 1, 1d } } }
                    }
                },
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1d } } }
            };
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, new SnapshotSetting(snapshotSettingName, new string[] { "test4" }) }
            };
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingName, new CategorySetting(categorySettingName, snapshotSettings) }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { categorySettingName, new DataPointSetting() },
                { snapshotSettingName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            Assert.Equal(0, result.Keys.Count);
        }

        [Fact]
        public void Create_UsesCategoryCreatedFromCategorySubTableDataAsOutputDictionaryKey_WhenPassedValidLuaTable()
        {
            var categorySettingName = "test1";
            var categorySubTableName = "test2";
            var snapshotSettingName = "test3";
            var dataPointTypeName = "test4";
            var categorySubTableTimestamp = 1d;
            var luaTable = new Dictionary<object, object>()
            {
                { categorySettingName, new Dictionary<object, object>()
                    { { categorySubTableName, new Dictionary<object, object>() { { 1,
                            categorySubTableTimestamp } } } }
                },
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1d } } },
                { dataPointTypeName, new Dictionary<object, object>() { { 1, 1d } } }
            };
            var snapshotSetting =  new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { categorySettingName, new DataPointSetting() },
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            var expectedCategory = new Category(categorySubTableName, categorySetting,
                    new List<TimestampRange>()
                    {
                        new TimestampRange(categorySubTableTimestamp, double.MaxValue)
                    });
            var key = result.Keys.ToList()[0];
            Assert.Contains(expectedCategory.ActiveTimePeriods[0], key.ActiveTimePeriods);
            Assert.Equal(expectedCategory.Identifier, key.Identifier);
            Assert.Equal(expectedCategory.Setting, key.Setting);
        }

        [Fact]
        public void Create_UsesInputSnapshotSettingAsOutputSubDictionaryKey_WhenPassedValidLuaTable()
        {
            var categorySettingName = "test1";
            var categorySubTableName = "test2";
            var snapshotSettingName = "test3";
            var dataPointTypeName = "test4";
            var categorySubTableTimestamp = 1d;
            var luaTable = new Dictionary<object, object>()
            {
                { categorySettingName, new Dictionary<object, object>()
                    { { categorySubTableName, new Dictionary<object, object>() { { 1,
                            categorySubTableTimestamp } } } }
                },
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1d } } },
                { dataPointTypeName, new Dictionary<object, object>() { { 1, 1d } } }
            };
            var snapshotSetting =  new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { categorySettingName, new DataPointSetting() },
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            var key = result.Keys.ToList()[0];
            Assert.Contains(snapshotSetting, result[key].Keys);
        }

        [Theory]
        [InlineData(1d)]
        [InlineData(1d, 2d)]
        [InlineData(1d, 2d, 3d, 4d, 5d)]
        public void Create_ReturnsASnapshotForEachEntryOfASnapshotSetting_WhenPassedALuaTableWithEntriesForAsnapshotSetting(
                params double[] timestamps)
        {
            var categorySettingName = "test1";
            var categorySubTableName = "test2";
            var snapshotSettingName = "test3";
            var dataPointTypeName = "test4";
            var categorySubTableTimestamp = 1d;
            var subTableTimestamps = new Dictionary<object, object>();
            for(int i = 0; i < timestamps.Length; i++)
            {
                subTableTimestamps[i + 1] = timestamps[i];
            }
            var luaTable = new Dictionary<object, object>()
            {
                { categorySettingName, new Dictionary<object, object>()
                    { { categorySubTableName, new Dictionary<object, object>() { { 1,
                            categorySubTableTimestamp } } } }
                },
                { snapshotSettingName, subTableTimestamps },
                { dataPointTypeName, new Dictionary<object, object>() { { 1, 1d } } }
            };
            var snapshotSetting = new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { categorySettingName, new DataPointSetting() },
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            var key = result.Keys.ToList()[0];
            var snapshots = result[key][snapshotSetting];
            Assert.Equal(timestamps.Length, snapshots.Count);
        }

        [Theory]
        [InlineData("test1")]
        [InlineData("test1", "test2")]
        [InlineData("test1", "test2", "test3", "test4", "test5")]
        public void Create_ReturnsASnapshotForEachEntryOfASnapshotSetting_WhenSnapshotEntryDataIsNested(
                params string[] subTableNames)
        {
            var categorySettingName = "test1";
            var categorySubTableName = "test2";
            var snapshotSettingName = "test3";
            var dataPointTypeName = "test4";
            var categorySubTableTimestamp = 1d;
            var entriesPerSubTable = 5;
            var subTables = new Dictionary<object, object>();
            for(int i = 0; i < subTableNames.Length; i++)
            {
                var subTableTimestamps = new Dictionary<object, object>();
                for(int j = 0; j < entriesPerSubTable; j++)
                {
                    subTableTimestamps[j + 1] = (double)(i + 1) + (j + 1);
                }

                subTables[subTableNames[i]] = subTableTimestamps;
            }
            var luaTable = new Dictionary<object, object>()
            {
                { categorySettingName, new Dictionary<object, object>()
                    { { categorySubTableName, new Dictionary<object, object>() { { 1,
                            categorySubTableTimestamp } } } }
                },
                { snapshotSettingName, subTables },
                { dataPointTypeName, new Dictionary<object, object>() { { 1, 1d } } }
            };
            var snapshotSetting = new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { categorySettingName, new DataPointSetting() },
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            var key = result.Keys.ToList()[0];
            var snapshots = result[key][snapshotSetting];
            Assert.Equal(subTableNames.Length * entriesPerSubTable, snapshots.Count);
        }

        [Fact]
        public void Create_AddsEntriesOfDataPointTypesToTheSnapshot_WhenEntriesForADataPointTypeAreNotNested()
        {
            var categorySettingName = "test1";
            var categorySubTableName = "test2";
            var snapshotSettingName = "test3";
            var dataPointTypeName = "test4";
            var categorySubTableTimestamp = 1d;
            var luaTable = new Dictionary<object, object>()
            {
                { categorySettingName, new Dictionary<object, object>()
                    { { categorySubTableName, new Dictionary<object, object>() { { 1,
                            categorySubTableTimestamp } } } }
                },
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1d } } },
                { dataPointTypeName, new Dictionary<object, object>() { { 1, 1d } } }
            };
            var snapshotSetting = new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { categorySettingName, new DataPointSetting() },
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            var key = result.Keys.ToList()[0];
            var matchingDataPoints = result[key][snapshotSetting][0].DataPointsIds
                    .Where(dp => dp.Contains(dataPointTypeName))
                    .ToList();
            Assert.Equal(1, matchingDataPoints.Count);
        }

        [Fact]
        public void Create_AddsEntriesOfDataPointTypesToTheSnapshot_WhenEntriesForADataPointTypeAreNested()
        {
            var categorySettingName = "test1";
            var categorySubTableName = "test2";
            var snapshotSettingName = "test3";
            var dataPointTypeName = "test4";
            var categorySubTableTimestamp = 1d;
            var subTable = new Dictionary<object, object>() { { 1, 1d } };
            var luaTable = new Dictionary<object, object>()
            {
                { categorySettingName, new Dictionary<object, object>()
                    { { categorySubTableName, new Dictionary<object, object>() { { 1,
                            categorySubTableTimestamp } } } }
                },
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1d } } },
                { dataPointTypeName, new Dictionary<object, object>() { { "test", subTable } } }
            };
            var snapshotSetting = new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { categorySettingName, new DataPointSetting() },
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            var key = result.Keys.ToList()[0];
            var matchingDataPoints = result[key][snapshotSetting][0].DataPointsIds
                    .Where(dp => dp.Contains(dataPointTypeName))
                    .ToList();
            Assert.Equal(1, matchingDataPoints.Count);
        }

        [Fact]
        public void Create_DoesNotAddEntryOfDataPointType_WhenDataPointTypeIsInSnapshotSettingButNoDataForItExists()
        {
            var categorySettingName = "test1";
            var categorySubTableName = "test2";
            var snapshotSettingName = "test3";
            var dataPointTypeName = "test4";
            var categorySubTableTimestamp = 1d;
            var luaTable = new Dictionary<object, object>()
            {
                { categorySettingName, new Dictionary<object, object>()
                    { { categorySubTableName, new Dictionary<object, object>() { { 1,
                            categorySubTableTimestamp } } } }
                },
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1d } } },
            };
            var snapshotSetting = new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { categorySettingName, new DataPointSetting() },
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            Assert.Equal(0, result.Keys.Count);
        }

        [Theory]
        [InlineData(1d, 2d, 3d)]
        [InlineData(1d, 1.5d, 3d)]
        [InlineData(1d)]
        public void Create_AddsDataPointWithRangeThatSnapshotTimestampFallsBetween_WhenADataPointTimestampWithAStartTimeLessThanTheSnapshotTimestampExists(
                params double[] dataPointTimestamps)
        {
            var categorySettingName = "test1";
            var categorySubTableName = "test2";
            var snapshotSettingName = "test3";
            var dataPointTypeName = "test4";
            var categorySubTableTimestamp = 1d;
            var snapshotTimeStamp = 2d;
            var subTable = new Dictionary<object, object>();
            for(int i = 0; i < dataPointTimestamps.Length; i++)
            {
                subTable["subTable" + i.ToString()] = new Dictionary<object, object>()
                {
                    { i + 1, dataPointTimestamps[i] }
                };
            }
            var luaTable = new Dictionary<object, object>()
            {
                { categorySettingName, new Dictionary<object, object>()
                    { { categorySubTableName, new Dictionary<object, object>() { { 1,
                            categorySubTableTimestamp } } } }
                },
                { snapshotSettingName, new Dictionary<object, object>() { { 1, snapshotTimeStamp } } },
                { dataPointTypeName, subTable }
            };
            var snapshotSetting = new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { categorySettingName, new DataPointSetting() },
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            var correctSubTableName = "subTable0";
            var correctTimeStamp = dataPointTimestamps[0];
            for(int i = 1; i < dataPointTimestamps.Length - 1; i++)
            {
                var timestamp = dataPointTimestamps[i];
                var nextTimestamp = dataPointTimestamps[i + 1];

                if(timestamp > correctTimeStamp
                        && timestamp <= snapshotTimeStamp
                        && nextTimestamp > snapshotTimeStamp)
                {
                    correctSubTableName = "subTable" + i;
                    correctTimeStamp = timestamp;
                }
            }
            var key = result.Keys.ToList()[0];
            var matchingDataPoints = result[key][snapshotSetting][0].DataPointsIds
                    .Where(dp => dp.Contains(correctSubTableName))
                    .ToList();
            Assert.Equal(1, matchingDataPoints.Count);
        }

        [Fact]
        public void Create_AddsMultipleDataPoints_WhenMoreThanOneForADataTypeHasARangeThatTheSnapshotTimestampFallsBetween()
        {
            var categorySettingName = "test1";
            var categorySubTableName = "test2";
            var snapshotSettingName = "test3";
            var dataPointTypeName = "test4";
            var categorySubTableTimestamp = 1d;
            var trueSubTable = new Dictionary<object, object>() { { 1, 1d } };
            var falseSubTable = new Dictionary<object, object>() { { 1, 3d } };
            var snapshotTimeStamp = 2d;
            var luaTable = new Dictionary<object, object>()
            {
                { categorySettingName, new Dictionary<object, object>()
                    { { categorySubTableName, new Dictionary<object, object>() { { 1,
                            categorySubTableTimestamp } } } }
                },
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
            var categorySetting = new CategorySetting(categorySettingName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { categorySettingName, new DataPointSetting() },
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting("false", 1) }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            var key = result.Keys.ToList()[0];
            Assert.Equal(2, result[key][snapshotSetting][0].DataPointsIds.Count);
        }

        [Theory]
        [InlineData(3d)]
        [InlineData(3d, 4.5d, 5d)]
        public void Create_DoesNotCreateASnapshot_WhenAllDataPointTimestampsAreLessThanEqualToSnapshotTimestamp(
                params double[] dataPointTimestamps)
        {
            var categorySettingName = "test1";
            var categorySubTableName = "test2";
            var snapshotSettingName = "test3";
            var dataPointTypeName = "test4";
            var categorySubTableTimestamp = 1d;
            var snapshotTimeStamp = 2d;
            var subTable = new Dictionary<object, object>();
            for(int i = 0; i < dataPointTimestamps.Length; i++)
            {
                subTable[i + 1] = dataPointTimestamps[i];
            }
            var luaTable = new Dictionary<object, object>()
            {
                { categorySettingName, new Dictionary<object, object>()
                    { { categorySubTableName, new Dictionary<object, object>() { { 1,
                            categorySubTableTimestamp } } } }
                },
                { snapshotSettingName, new Dictionary<object, object>() { { 1, snapshotTimeStamp } } },
                { dataPointTypeName, new Dictionary<object, object>() { { "test", subTable } } }
            };
            var snapshotSetting = new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { categorySettingName, new DataPointSetting() },
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            Assert.Equal(0, result.Keys.Count);
        }

        [Fact]
        public void Create_AssignsConcatenatedTypeNameAndSubTypeNameAsDataPointIds_WhenPassedALuaTableWithEntriesForAsnapshotSetting()
        {
            var categorySettingName = "test1";
            var categorySubTableName = "test2";
            var snapshotSettingName = "test3";
            var dataPointTypeName = "test4";
            var dataPointSubTypeName = "test5";
            var luaTable = new Dictionary<object, object>()
            {
                { categorySettingName, new Dictionary<object, object>()
                    { { categorySubTableName, new Dictionary<object, object>() { { 1,
                            1d } } } }
                },
                { snapshotSettingName, new Dictionary<object, object>() { { 1, 1d } } },
                { dataPointTypeName, new Dictionary<object, object>()
                    {
                        {
                            dataPointSubTypeName, new Dictionary<object, object>() { { 1, 1d } }
                        }
                    }
                }
            };
            var snapshotSetting = new SnapshotSetting(snapshotSettingName, new string[] { dataPointTypeName });
            var snapshotSettings = new Dictionary<string, SnapshotSetting>
            {
                { snapshotSettingName, snapshotSetting }
            };
            var categorySetting = new CategorySetting(categorySettingName, snapshotSettings);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { categorySettingName, categorySetting }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { categorySettingName, new DataPointSetting() },
                { snapshotSettingName, new DataPointSetting() },
                { dataPointTypeName, new DataPointSetting() }
            };

            var result = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

            var key = result.Keys.ToList()[0];
            var expectedId = dataPointTypeName + "__" + dataPointSubTypeName;
            Assert.Equal(expectedId, result[key][snapshotSetting][0].DataPointsIds[0]);
        }
    }
}
