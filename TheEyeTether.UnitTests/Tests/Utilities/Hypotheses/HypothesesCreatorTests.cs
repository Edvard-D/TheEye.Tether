using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using TheEye.Tether.Data;
using TheEye.Tether.UnitTests.Stubs;
using TheEye.Tether.Utilities.Hypotheses;
using Xunit;

namespace TheEye.Tether.UnitTests.Tests.Utilities.Hypotheses
{
	public class HypothesesCreatorTests
	{
		private const string CategoryId = "CategoryId";
		private const string CategoryType = "CategoryType";
		private const string CurrentDomainBaseDirectory = @"C:\TheEyeTether\";
		private const string DateTimeFormat = "yyyy_MM_dd__HH_mm_ss";
		private const int MinRequiredSnapshots = 20;
		private const string NowDateTimeString = "2021_09_23__12_00_00";
		private const string SnapshotId = "SnapshotId";
		private const string SnapshotType = "SnapshotType";


		private string CreateDirectoryPath(string snapshotIdModfiier="")
		{
			var directoryPathElements = new string[]
			{
				CurrentDomainBaseDirectory,
				"Data",
				"Snapshots",
				CategoryType,
				CategoryId,
				SnapshotType,
				SnapshotId + snapshotIdModfiier
			};
			
			return Path.Combine(directoryPathElements) + Path.DirectorySeparatorChar;
		}

		[Fact]
		public void Create_ReturnsListOfHypotheses_WhenCalled()
		{
			var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
					.ToUniversalTime();
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ CurrentDomainBaseDirectory + "test.json", new MockFileData(string.Empty) }
			});
			var stubClock = new StubClock(nowDateTime);
			var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);

			var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

			Assert.IsType<List<Hypothesis>>(result);
		}

		[Fact]
		public void Create_DoesNotThrowException_WhenNoSnapshotsExist()
		{
			var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
					.ToUniversalTime();
			var mockFileSystem = new MockFileSystem();
			var stubClock = new StubClock(nowDateTime);
			var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);

			var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

			Assert.True(true);
		}

		[Fact]
		public void Create_DeletesFile_WhenOlderThan7Days()
		{
			var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
					.ToUniversalTime();
			var creationDateTime = nowDateTime.AddDays(-8);
			var fileName = creationDateTime.ToString(DateTimeFormat) + ".json";
			var mockFileData = new MockFileData(string.Empty);
			mockFileData.CreationTime = creationDateTime;
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ CreateDirectoryPath() + fileName, mockFileData }
			});
			var stubClock = new StubClock(nowDateTime);
			var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);

			var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

			Assert.DoesNotContain(CreateDirectoryPath() + fileName, mockFileSystem.AllFiles);
		}

		[Fact]
		public void Create_CreatesHypothesis_WhenGreaterThanEqualTo20SnapshotsExistForTypeAndSubType()
		{
			var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
					.ToUniversalTime();
			var fileName = nowDateTime.ToString(DateTimeFormat) + ".json";
			var snapshots = new List<List<string>>();
			for(int i = 0; i < MinRequiredSnapshots; i++)
			{
				snapshots.Add(new List<string>() { "test" });
			}
			var fileDataJson = JsonSerializer.Serialize(snapshots);
			var mockFileData = new MockFileData(fileDataJson);
			mockFileData.CreationTime = nowDateTime;
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ CreateDirectoryPath() + fileName, mockFileData }
			});
			var stubClock = new StubClock(nowDateTime);
			var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);

			var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

			Assert.NotEmpty(result);
		}
		
		[Fact]
		public void Create_DoesNotCreateHypothesis_WhenLessThan20SnapshotsExistForTypeAndSubType()
		{
			var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
					.ToUniversalTime();
			var fileName = nowDateTime.ToString(DateTimeFormat) + ".json";
			var snapshots = new List<List<string>>();
			for(int i = 0; i < MinRequiredSnapshots - 1; i++)
			{
				snapshots.Add(new List<string>() { "test" });
			}
			var fileDataJson = JsonSerializer.Serialize(snapshots);
			var mockFileData = new MockFileData(fileDataJson);
			mockFileData.CreationTime = nowDateTime;
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ CreateDirectoryPath() + fileName, mockFileData }
			});
			var stubClock = new StubClock(nowDateTime);
			var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);

			var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

			Assert.Empty(result);
		}

		[Fact]
		public void Create_GroupsDataPointStrings_WhenTheyCorrelate()
		{
			var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
					.ToUniversalTime();
			var fileName = nowDateTime.ToString(DateTimeFormat) + ".json";
			var testDataPointString1 = "testDataPointString1";
			var testDataPointString2 = "testDataPointString2";
			var snapshots = new List<List<string>>();
			for(int i = 0; i < MinRequiredSnapshots; i++)
			{
				snapshots.Add(new List<string>() { testDataPointString1 });
			}
			for(int i = 0; i < (int)(MinRequiredSnapshots * 0.75); i++)
			{
				snapshots[i].Add(testDataPointString2);
			}
			var fileDataJson = JsonSerializer.Serialize(snapshots);
			var mockFileData = new MockFileData(fileDataJson);
			mockFileData.CreationTime = nowDateTime;
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ CreateDirectoryPath() + fileName, mockFileData }
			});
			var stubClock = new StubClock(nowDateTime);
			var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);

			var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);
			
			var dataPointStrings = new HashSet<string>()
			{
				testDataPointString1,
				testDataPointString2
			};
			Assert.Contains(result, h => h.DataPointStrings.SetEquals(dataPointStrings));
		}

		[Fact]
		public void Create_CreatesMultipleHypotheses_WhenMultipleHighlyCorrelatedDataPointStringsExist()
		{
			var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
					.ToUniversalTime();
			var fileName = nowDateTime.ToString(DateTimeFormat) + ".json";
			var testDataPointString1 = "testDataPointString1";
			var testDataPointString2 = "testDataPointString2";
			var testDataPointString3 = "testDataPointString3";
			var snapshots = new List<List<string>>();
			for(int i = 0; i < MinRequiredSnapshots; i++)
			{
				snapshots.Add(new List<string>() { testDataPointString1 });
			}
			for(int i = 0; i < (int)(MinRequiredSnapshots * 0.95f); i++)
			{
				snapshots[i].Add(testDataPointString2);
			}
			for(int i = 0; i < (int)(MinRequiredSnapshots * 0.85f); i++)
			{
				snapshots[i].Add(testDataPointString3);
			}
			var fileDataJson = JsonSerializer.Serialize(snapshots);
			var mockFileData = new MockFileData(fileDataJson);
			mockFileData.CreationTime = nowDateTime;
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ CreateDirectoryPath() + fileName, mockFileData }
			});
			var stubClock = new StubClock(nowDateTime);
			var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);

			var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

			var hashSet1 = new HashSet<string>() { testDataPointString1 };
			var hashSet2 = new HashSet<string>() { testDataPointString1, testDataPointString2 };
			var hashSet3 = new HashSet<string>() { testDataPointString1, testDataPointString2,
					testDataPointString3 };
			Assert.Contains(result, h => h.DataPointStrings.SetEquals(hashSet1));
			Assert.Contains(result, h => h.DataPointStrings.SetEquals(hashSet2));
			Assert.Contains(result, h => h.DataPointStrings.SetEquals(hashSet3));
		}

		[Fact]
		public void Create_DoesNotCreateHypotheses_WhenThereAreNoSnapshotsWhereTheCombinationOfDataPointStringsAppears()
		{
			var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
					.ToUniversalTime();
			var fileName = nowDateTime.ToString(DateTimeFormat) + ".json";
			var testDataPointString1 = "testDataPointString1";
			var testDataPointString2 = "testDataPointString2";
			var testDataPointString3 = "testDataPointString3";
			var snapshots = new List<List<string>>();
			for(int i = 0; i < MinRequiredSnapshots; i++)
			{
				snapshots.Add(new List<string>() { testDataPointString1 });
			}
			for(int i = 0; i < (int)(MinRequiredSnapshots* 0.95f); i++)
			{
				snapshots[i].Add(testDataPointString2);
			}
			for(int i = 0; i < (int)(MinRequiredSnapshots * 0.85f); i++)
			{
				snapshots[i].Add(testDataPointString3);
			}
			var fileDataJson = JsonSerializer.Serialize(snapshots);
			var mockFileData = new MockFileData(fileDataJson);
			mockFileData.CreationTime = nowDateTime;
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ CreateDirectoryPath() + fileName, mockFileData }
			});
			var stubClock = new StubClock(nowDateTime);
			var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);

			var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);
			
			var invalidHashSet1 = new HashSet<string>() { testDataPointString2 };
			var invalidHashSet2 = new HashSet<string>() { testDataPointString3 };
			var invalidHashSet3 = new HashSet<string>() { testDataPointString1, testDataPointString3 };
			Assert.DoesNotContain(result, h => h.DataPointStrings.SetEquals(invalidHashSet1));
			Assert.DoesNotContain(result, h => h.DataPointStrings.SetEquals(invalidHashSet2));
			Assert.DoesNotContain(result, h => h.DataPointStrings.SetEquals(invalidHashSet3));
		}

		[Fact]
		public void Create_PrioritizesCreatingGroupsWithHigherOverallTrueValues_WhenTwoPossibleGroupsCombinationsExistWithSimilarDistances()
		{
			var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
					.ToUniversalTime();
			var fileName = nowDateTime.ToString(DateTimeFormat) + ".json";
			var testDataPointString1 = "testDataPointString1";
			var testDataPointString2 = "testDataPointString2";
			var testDataPointString3 = "testDataPointString3";
			var snapshots = new List<List<string>>();
			for(int i = 0; i < MinRequiredSnapshots; i++)
			{
				snapshots.Add(new List<string>() { testDataPointString1 });
			}
			for(int i = 0; i < (int)(MinRequiredSnapshots * 0.95f); i++)
			{
				snapshots[i].Add(testDataPointString2);
			}
			for(int i = 0; i < (int)(MinRequiredSnapshots * 0.9f); i++)
			{
				snapshots[i].Add(testDataPointString3);
			}
			var fileDataJson = JsonSerializer.Serialize(snapshots);
			var mockFileData = new MockFileData(fileDataJson);
			mockFileData.CreationTime = nowDateTime;
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ CreateDirectoryPath() + fileName, mockFileData }
			});
			var stubClock = new StubClock(nowDateTime);
			var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);

			var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

			var validHashSet = new HashSet<string>() { testDataPointString1, testDataPointString2 };
			var invalidHashSet = new HashSet<string>() { testDataPointString2, testDataPointString3 };
			Assert.Contains(result, h => h.DataPointStrings.SetEquals(validHashSet));
			Assert.DoesNotContain(result, h => h.DataPointStrings.SetEquals(invalidHashSet));
		}
		
		[Fact]
		public void Create_OnlyLoadsSnapshotFilesInCurrentDomainBaseDirectoryDataSnapshots_WhenCalled()
		{
			var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
					.ToUniversalTime();
			var fileCreationTime1 = nowDateTime;
			var fileCreationTime2 = nowDateTime.AddDays(-1);
			var fileName1 = fileCreationTime1.ToString(DateTimeFormat) + ".json";
			var fileName2 = fileCreationTime2.ToString(DateTimeFormat) + ".json";
			var testDataPointString1 = "testDataPointString1";
			var testDataPointString2 = "testDataPointString2";
			var snapshots1 = new List<List<string>>();
			var snapshots2 = new List<List<string>>();
			for(int i = 0; i < MinRequiredSnapshots; i++)
			{
				snapshots1.Add(new List<string>() { testDataPointString1 });
				snapshots2.Add(new List<string>() { testDataPointString2 });
			}
			var fileDataJson1 = JsonSerializer.Serialize(snapshots1);
			var fileDataJson2 = JsonSerializer.Serialize(snapshots2);
			var mockFileData1 = new MockFileData(fileDataJson1);
			var mockFileData2 = new MockFileData(fileDataJson2);
			mockFileData1.CreationTime = fileCreationTime1;
			mockFileData2.CreationTime = fileCreationTime2;
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ @"C:\" + fileName1, mockFileData1 },
				{ CreateDirectoryPath() + fileName2, mockFileData2 }
			});
			var stubClock = new StubClock(nowDateTime);
			var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);

			var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

			Assert.DoesNotContain(result, h => h.DataPointStrings.Contains(testDataPointString1));
			Assert.Contains(result, h => h.DataPointStrings.Contains(testDataPointString2));
		}

		[Fact]
		public void Create_GetsAndAssignsCategoryTypeValueFromFilePath_WhenCalled()
		{
			var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
					.ToUniversalTime();
			var fileName = nowDateTime.ToString(DateTimeFormat) + ".json";
			var testDataPointString = "testDataPointString";
			var snapshots = new List<List<string>>();
			for(int i = 0; i < MinRequiredSnapshots; i++)
			{
				snapshots.Add(new List<string>() { testDataPointString });
			}
			var fileDataJson = JsonSerializer.Serialize(snapshots);
			var mockFileData = new MockFileData(fileDataJson);
			mockFileData.CreationTime = nowDateTime;
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ CreateDirectoryPath() + fileName, mockFileData }
			});
			var stubClock = new StubClock(nowDateTime);
			var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);

			var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

			Assert.Equal(CategoryType, result[0].CategoryType);
		}

		[Fact]
		public void Create_GetsAndAssignsCategoryIdValueFromFilePath_WhenCalled()
		{
			var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
					.ToUniversalTime();
			var fileName = nowDateTime.ToString(DateTimeFormat) + ".json";
			var testDataPointString = "testDataPointString";
			var snapshots = new List<List<string>>();
			for(int i = 0; i < MinRequiredSnapshots; i++)
			{
				snapshots.Add(new List<string>() { testDataPointString });
			}
			var fileDataJson = JsonSerializer.Serialize(snapshots);
			var mockFileData = new MockFileData(fileDataJson);
			mockFileData.CreationTime = nowDateTime;
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ CreateDirectoryPath() + fileName, mockFileData }
			});
			var stubClock = new StubClock(nowDateTime);
			var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);

			var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

			Assert.Equal(CategoryId, result[0].CategoryId);
		}

		[Fact]
		public void Create_GetsAndAssignsSnapshotTypeValueFromFilePath_WhenCalled()
		{
			var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
					.ToUniversalTime();
			var fileName = nowDateTime.ToString(DateTimeFormat) + ".json";
			var testDataPointString = "testDataPointString";
			var snapshots = new List<List<string>>();
			for(int i = 0; i < MinRequiredSnapshots; i++)
			{
				snapshots.Add(new List<string>() { testDataPointString });
			}
			var fileDataJson = JsonSerializer.Serialize(snapshots);
			var mockFileData = new MockFileData(fileDataJson);
			mockFileData.CreationTime = nowDateTime;
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ CreateDirectoryPath() + fileName, mockFileData }
			});
			var stubClock = new StubClock(nowDateTime);
			var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);

			var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

			Assert.Equal(SnapshotType, result[0].SnapshotType);
		}

		[Fact]
		public void Create_GetsAndAssignsSnapshotIdValueFromFilePath_WhenCalled()
		{
			var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
					.ToUniversalTime();
			var fileName = nowDateTime.ToString(DateTimeFormat) + ".json";
			var testDataPointString = "testDataPointString";
			var snapshots = new List<List<string>>();
			for(int i = 0; i < MinRequiredSnapshots; i++)
			{
				snapshots.Add(new List<string>() { testDataPointString });
			}
			var fileDataJson = JsonSerializer.Serialize(snapshots);
			var mockFileData = new MockFileData(fileDataJson);
			mockFileData.CreationTime = nowDateTime;
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ CreateDirectoryPath() + fileName, mockFileData }
			});
			var stubClock = new StubClock(nowDateTime);
			var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);

			var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

			Assert.Equal(SnapshotId, result[0].SnapshotId);
		}
	}
}
