using System.Collections.Generic;
using System.Linq;
using TheEye.Tether.Data;
using TheEye.Tether.Utilities.Hypotheses;
using Xunit;

namespace TheEye.Tether.UnitTests.Tests.Utilities.Hypotheses
{
	public class DataPointsCreatorTests
	{
		[Fact]
		public void Create_ReturnsDictionaryOfListsOfDataPoints_WhenPassedValidLuaTable()
		{
			var tableName = "test";
			var luaTable = new Dictionary<object, object>()
			{
				{ tableName, new Dictionary<object, object>() { { 1L, 1d } } }
			};
			var dataPointSettings = new Dictionary<string, DataPointSetting>()
			{
				{ tableName, new DataPointSetting() }
			};

			var result = DataPointsCreator.Create(luaTable, dataPointSettings);

			Assert.IsType<Dictionary<string, List<DataPoint>>>(result);
		}

		[Theory]
		[InlineData("test1")]
		[InlineData("test1", "test2")]
		[InlineData("test1", "test2", "test3", "test4", "test5")]
		public void Create_CreatesADictionaryEntryForEachFirstLevelSubTable_WhenPassedValidLuaTable(
				params string[] tableNames)
		{
			var luaTable = new Dictionary<object, object>();
			var dataPointSettings = new Dictionary<string, DataPointSetting>();
			foreach(string tableName in tableNames)
			{
				luaTable[tableName] = new Dictionary<object, object>() { { 1L, 1d } };
				dataPointSettings[tableName] = new DataPointSetting();
			}

			var result = DataPointsCreator.Create(luaTable, dataPointSettings);

			Assert.Equal(tableNames.Length, result.Keys.Count);
		}

		[Fact]
		public void Create_UsesInputTableKeysAsOutputDictionaryKeys_WhenPassedValidLuaTable()
		{
			var tableName = "test";
			var luaTable = new Dictionary<object, object>()
			{
				{ tableName, new Dictionary<object, object>() { { 1L, 1d } } }
			};
			var dataPointSettings = new Dictionary<string, DataPointSetting>()
			{
				{ tableName, new DataPointSetting() }
			};

			var result = DataPointsCreator.Create(luaTable, dataPointSettings);

			Assert.Contains(tableName, result.Keys);
		}

		[Theory]
		[InlineData(1d)]
		[InlineData(1d, 2d)]
		[InlineData(1d, 2d, 3d, 4d, 5d)]
		public void Create_ReturnsADataPointForEachDataEntry_WhenEntriesAreNotNested(
				params double[] timestamps)
		{
			var subTable = new Dictionary<object, object>();
			for(int i = 0; i < timestamps.Length; i++)
			{
				subTable[(long)i + 1L] = timestamps[i];
			}
			var tableName = "test";
			var luaTable = new Dictionary<object, object>()
			{
				{ tableName, subTable }
			};
			var dataPointSettings = new Dictionary<string, DataPointSetting>()
			{
				{ tableName, new DataPointSetting() }
			};

			var result = DataPointsCreator.Create(luaTable, dataPointSettings);

			Assert.Equal(timestamps.Length, result[tableName].Count);
		}

		[Theory]
		[InlineData(1d)]
		[InlineData(1d, 2d)]
		[InlineData(1d, 2d, 3d, 4d, 5d)]
		public void Create_ReturnsADataPointForEachDataEntry_WhenEntriesAreNested(
				params double[] timestamps)
		{
			var subTable = new Dictionary<object, object>();
			for(int i = 0; i < timestamps.Length; i++)
			{
				subTable[(long)i + 1L] = timestamps[i];
			}
			var tableName = "test1";
			var subTableName = "test2";
			var luaTable = new Dictionary<object, object>()
			{
				{ tableName, new Dictionary<object, object>() { { subTableName, subTable } } }
			};
			var dataPointSettings = new Dictionary<string, DataPointSetting>()
			{
				{ tableName, new DataPointSetting() }
			};

			var result = DataPointsCreator.Create(luaTable, dataPointSettings);

			Assert.Equal(timestamps.Length, result[tableName].Count);
		}

		[Fact]
		public void Create_AssignsTopLevelTableKeyAsDataPointTypeName_WhenPassedValidLuaTable()
		{
			var tableName = "test";
			var luaTable = new Dictionary<object, object>()
			{
				{ tableName, new Dictionary<object, object>() { { 1L, 1d } } }
			};
			var dataPointSettings = new Dictionary<string, DataPointSetting>()
			{
				{ tableName, new DataPointSetting() }
			};

			var result = DataPointsCreator.Create(luaTable, dataPointSettings);

			Assert.Equal(tableName, result[tableName][0].TypeName);
		}

		[Fact]
		public void Create_AssignsNullAsDataPointSubTypeName_WhenEntiresAreNotNested()
		{
			var tableName = "test";
			var luaTable = new Dictionary<object, object>()
			{
				{ tableName, new Dictionary<object, object>() { { 1L, 1d } } }
			};
			var dataPointSettings = new Dictionary<string, DataPointSetting>()
			{
				{ tableName, new DataPointSetting() }
			};

			var result = DataPointsCreator.Create(luaTable, dataPointSettings);

			Assert.Null(result[tableName][0].SubTypeName);
		}

		[Fact]
		public void Create_AssignsSubTableKeyAsDataPointSubTypeName_WhenEntriesAreNested()
		{
			var tableName = "test1";
			var subTableName = "test2";
			var subTable = new Dictionary<object, object>() { { 1L, 1d } };
			var luaTable = new Dictionary<object, object>()
			{
				{ tableName, new Dictionary<object, object>() { { subTableName, subTable } } }
			};
			var dataPointSettings = new Dictionary<string, DataPointSetting>()
			{
				{ tableName, new DataPointSetting() }
			};

			var result = DataPointsCreator.Create(luaTable, dataPointSettings);

			Assert.Equal(subTableName, result[tableName][0].SubTypeName);
		}

		[Fact]
		public void Create_AssignsDataEntryValueAsDataPointTimestampRangeStart_WhenPassedValidLuaTable()
		{
			var tableName = "test";
			var timestamp = 1d;
			var luaTable = new Dictionary<object, object>()
			{
				{ tableName, new Dictionary<object, object>() { { 1L, timestamp } } }
			};
			var dataPointSettings = new Dictionary<string, DataPointSetting>()
			{
				{ tableName, new DataPointSetting() }
			};

			var result = DataPointsCreator.Create(luaTable, dataPointSettings);

			Assert.Equal(timestamp, result[tableName][0].TimestampRange.Start);
		}

		[Theory]
		[InlineData(1d, 2d)]
		[InlineData(2d, 1d)]
		[InlineData(1d, 2d, 5d, 4d, 3d)]
		public void Create_AssignsNextLargestValueAsDataPointTimestampRangeEnd_WhenThereIsALargerTimestampAndDataPointSettingEndMarkerIsNull(
				params double[] timestamps)
		{
			var tableName = "test";
			var subTable = new Dictionary<object, object>();
			for(int i = 0; i < timestamps.Length; i++)
			{
				subTable[(long)i + 1L] = timestamps[i];
			}
			var luaTable = new Dictionary<object, object>()
			{
				{ tableName, subTable }
			};
			var dataPointSettings = new Dictionary<string, DataPointSetting>()
			{
				{ tableName, new DataPointSetting() }
			};

			var result = DataPointsCreator.Create(luaTable, dataPointSettings);

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
		[InlineData(1d, 2d)]
		[InlineData(2d, 1d)]
		[InlineData(1d, 2d, 5d, 4d, 3d)]
		public void Create_AssignsNextLargestValueAsDataPointTimestampRangeEnd_WhenThereIsALargerTimestampAndDataPointSettingEndMarkerIsEmptyString(
				params double[] timestamps)
		{
			var tableName = "test";
			var subTable = new Dictionary<object, object>();
			for(int i = 0; i < timestamps.Length; i++)
			{
				subTable[(long)i + 1L] = timestamps[i];
			}
			var luaTable = new Dictionary<object, object>()
			{
				{ tableName, subTable }
			};
			var dataPointSettings = new Dictionary<string, DataPointSetting>()
			{
				{ tableName, new DataPointSetting(string.Empty, -1, new int[0]) }
			};

			var result = DataPointsCreator.Create(luaTable, dataPointSettings);

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
		[InlineData(1d)]
		[InlineData(1d, 2d)]
		[InlineData(2d, 1d)]
		[InlineData(1d, 2d, 5d, 4d, 3d)]
		public void Create_AssignsMaxValueAsDataPointTimestampRangeEnd_WhenLargestTimestamp(
				params double[] timestamps)
		{
			var tableName = "test";
			var subTable = new Dictionary<object, object>();
			for(int i = 0; i < timestamps.Length; i++)
			{
				subTable[(long)i + 1L] = timestamps[i];
			}
			var luaTable = new Dictionary<object, object>()
			{
				{ tableName, subTable }
			};
			var dataPointSettings = new Dictionary<string, DataPointSetting>()
			{
				{ tableName, new DataPointSetting() }
			};

			var result = DataPointsCreator.Create(luaTable, dataPointSettings);

			var largestTimestamp = timestamps.ToList().Max();
			var largestTimestampDataPoint = result[tableName]
					.Where(dp => dp.TimestampRange.Start == largestTimestamp)
					.First();
			Assert.Equal(double.MaxValue, largestTimestampDataPoint.TimestampRange.End);
		}

		[Fact]
		public void Create_AssignsMatchingFalseTimestampAsDataPointTimestampRangeEnd_WhenValidMatchingSubTableExistsAndDataPointSettingsHasFalseAssigned()
		{
			var true1Timestamp = 1d;
			var true2Timestamp = 3d;
			var false1Timestamp = 2d;
			var false2Timestamp = 4d;
			var subTables = new Dictionary<object, object>()
			{
				{ "test1_true", new Dictionary<object, object>() { { 1L, true1Timestamp } } },
				{ "test1_false", new Dictionary<object, object>() { { 1L, false1Timestamp } } },
				{ "test2_true", new Dictionary<object, object>() { { 1L, true2Timestamp } } },
				{ "test2_false", new Dictionary<object, object>() { { 1L, false2Timestamp } } }
			};
			var tableName = "test";
			var luaTable = new Dictionary<object, object>()
			{
				{ tableName, subTables }
			};
			var dataPointSettings = new Dictionary<string, DataPointSetting>()
			{
				{ tableName, new DataPointSetting("false", 1, new int[] { 0 }) }
			};

			var result = DataPointsCreator.Create(luaTable, dataPointSettings);

			var matchingDataPoint = result[tableName]
					.Where(dp => dp.TimestampRange.Start == true1Timestamp)
					.First();
			Assert.Equal(false1Timestamp, matchingDataPoint.TimestampRange.End);
		}

		[Fact]
		public void Create_AssignsMaxValueAsDataPointTimestampRangeEnd_WhenValidMatchingSubTableDoesNotExist()
		{
			var timestamp = 1d;
			var subTable = new Dictionary<object, object>()
			{
				{ "test_true", new Dictionary<object, object>() { { 1L, timestamp } } }
			};
			var tableName = "test";
			var luaTable = new Dictionary<object, object>()
			{
				{ tableName, subTable }
			};
			var dataPointSettings = new Dictionary<string, DataPointSetting>()
			{
				{ tableName, new DataPointSetting("false", 1, new int[] { 0 }) }
			};

			var result = DataPointsCreator.Create(luaTable, dataPointSettings);

			var matchingDataPoint = result[tableName]
					.Where(dp => dp.TimestampRange.Start == timestamp)
					.First();
			Assert.Equal(double.MaxValue, matchingDataPoint.TimestampRange.End);
		}

		[Fact]
		public void Create_AssignsSameValueAsDataPointTimestampRangeStartAndEnd_WhenDataPointTypeIsASnapshotType()
		{
			var timestamp = 1d;
			var subTable = new Dictionary<object, object>()
			{
				{ "test_true", new Dictionary<object, object>() { { 1L, timestamp } } }
			};
			var tableName = "test1";
			var categorySettingName = "test2";
			var luaTable = new Dictionary<object, object>()
			{
				{ tableName, subTable }
			};
			var snapshotSettings = new Dictionary<string, SnapshotSetting>()
			{
				{ tableName, new SnapshotSetting(tableName, null) }
			};
			var categorySettings = new Dictionary<string, CategorySetting>()
			{
				{ categorySettingName, new CategorySetting(categorySettingName, snapshotSettings) }
			};
			var dataPointSettings = new Dictionary<string, DataPointSetting>()
			{
				{ tableName, new DataPointSetting() }
			};

			var result = DataPointsCreator.Create(luaTable, dataPointSettings, categorySettings);

			var matchingDataPoint = result[tableName][0];
			Assert.Equal(matchingDataPoint.TimestampRange.Start, matchingDataPoint.TimestampRange.End);
		}

		[Fact]
		public void Create_DoesNotAddEntryOfDataPointType_WhenItIsAnEndMarker()
		{
			var subTable = new Dictionary<object, object>()
			{
				{ "test_false", new Dictionary<object, object>() { { 1L, 1d } } }
			};
			var tableName = "test";
			var luaTable = new Dictionary<object, object>()
			{
				{ tableName, subTable }
			};
			var dataPointSettings = new Dictionary<string, DataPointSetting>()
			{
				{ tableName, new DataPointSetting("false", 1, new int[] { 0 }) }
			};

			var result = DataPointsCreator.Create(luaTable, dataPointSettings);

			Assert.Empty(result[tableName]);
		}

		[Fact]
		public void Create_AssignsMaxValueTimeStamp_WhenMultipleStartTimeStampsAreTheSameWithNoEndTimestamps()
		{
			var true1Timestamp = 1d;
			var true2Timestamp = 1d;
			var subTables = new Dictionary<object, object>()
			{
				{ "test1_true", new Dictionary<object, object>() { { 1L, true1Timestamp } } },
				{ "test2_true", new Dictionary<object, object>() { { 1L, true2Timestamp } } }
			};
			var tableName = "test";
			var luaTable = new Dictionary<object, object>()
			{
				{ tableName, subTables }
			};
			var dataPointSettings = new Dictionary<string, DataPointSetting>()
			{
				{ tableName, new DataPointSetting("false", 1, new int[] { 0 }) }
			};

			var result = DataPointsCreator.Create(luaTable, dataPointSettings);

			Assert.Equal(double.MaxValue, result[tableName][0].TimestampRange.End);
			Assert.Equal(double.MaxValue, result[tableName][1].TimestampRange.End);
		}

		[Fact]
		public void Create_AssignsCorrectEndTimeStamp_WhenThereAreMultipleSubTypeCategories()
		{
			var true1Timestamp = 1d;
			var true2Timestamp = 2d;
			var false1Timestamp = 3d;
			var false2Timestamp = 4d;
			var subTables = new Dictionary<object, object>()
			{
				{ "test1_true", new Dictionary<object, object>() { { 1L, true1Timestamp } } },
				{ "test2_true", new Dictionary<object, object>() { { 1L, true2Timestamp } } },
				{ "test1_false", new Dictionary<object, object>() { { 1L, false1Timestamp } } },
				{ "test2_false", new Dictionary<object, object>() { { 1L, false2Timestamp } } }
			};
			var typeName1 = "test1";
			var typeName2 = "test2";
			var luaTable = new Dictionary<object, object>()
			{
				{ typeName1, subTables },
				{ typeName2, subTables }
			};
			var dataPointSettings = new Dictionary<string, DataPointSetting>()
			{
				{ typeName1, new DataPointSetting("false", 1, new int[] { 0 }) },
				{ typeName2, new DataPointSetting(string.Empty, -1, new int[] { 0 }) }
			};

			var result = DataPointsCreator.Create(luaTable, dataPointSettings);

			var matchingDataPoint1 = result[typeName1]
					.Where(dp => dp.TimestampRange.Start == true1Timestamp)
					.First();
			var matchingDataPoint2 = result[typeName1]
					.Where(dp => dp.TimestampRange.Start == true2Timestamp)
					.First();
			var matchingDataPoint3 = result[typeName2]
					.Where(dp => dp.TimestampRange.Start == true1Timestamp)
					.First();
			var matchingDataPoint4 = result[typeName2]
					.Where(dp => dp.TimestampRange.Start == true2Timestamp)
					.First();
			Assert.Equal(false1Timestamp, matchingDataPoint1.TimestampRange.End);
			Assert.Equal(false2Timestamp, matchingDataPoint2.TimestampRange.End);
			Assert.Equal(false1Timestamp, matchingDataPoint3.TimestampRange.End);
			Assert.Equal(false2Timestamp, matchingDataPoint4.TimestampRange.End);
		}
		
		[Fact]
		public void Create_AssignsTimestampsInAscendingOrder_WhenTheyAreNotInAscendingOrderToStart()
		{
			var true1Timestamp = 1d;
			var true2Timestamp = 3d;
			var false1Timestamp = 2d;
			var false2Timestamp = 4d;
			var subTables = new Dictionary<object, object>()
			{
				{ "test_true", new Dictionary<object, object>() {
					{ 1L, true1Timestamp },
					{ 2L, true2Timestamp }
				} },
				{ "test_false", new Dictionary<object, object>() {
					{ 1L, false2Timestamp },
					{ 2L, false1Timestamp }
				} },
			};
			var tableName = "test";
			var luaTable = new Dictionary<object, object>()
			{
				{ tableName, subTables }
			};
			var dataPointSettings = new Dictionary<string, DataPointSetting>()
			{
				{ tableName, new DataPointSetting("false", 1, new int[] { 0 }) }
			};

			var result = DataPointsCreator.Create(luaTable, dataPointSettings);

			var matchingDataPoint1 = result[tableName]
					.Where(dp => dp.TimestampRange.Start == true1Timestamp)
					.First();
			var matchingDataPoint2 = result[tableName]
					.Where(dp => dp.TimestampRange.Start == true2Timestamp)
					.First();
			Assert.Equal(false1Timestamp, matchingDataPoint1.TimestampRange.End);
			Assert.Equal(false2Timestamp, matchingDataPoint2.TimestampRange.End);
		}
	}
}
