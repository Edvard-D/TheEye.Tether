using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text.Json;
using TheEyeTether.Types;
using TheEyeTether.UnitTests.Stubs;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class HypothesesCreatorTests
    {
        private const string CategoryId = "CategoryId";
        private const string CategoryType = "CategoryType";
        private const string CurrentDomainBaseDirectory = @"C:\TheEyeTether\";
        private const string DateTimeFormat = "yyyy_MM_dd__HH_mm_ss";
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
            var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryGetter(
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
            var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryGetter(
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
            var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryGetter(
                    CurrentDomainBaseDirectory);

            var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

            Assert.DoesNotContain(CreateDirectoryPath() + fileName, mockFileSystem.AllFiles);
        }

        [Fact]
        public void Create_CreatesHypothesis_WhenGreaterThanEqualTo100SnapshotsExistForTypeAndSubType()
        {
            var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
                    .ToUniversalTime();
            var fileName = nowDateTime.ToString(DateTimeFormat) + ".json";
            var snapshots = new List<List<string>>();
            for(int i = 0; i < 100; i++)
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
            var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryGetter(
                    CurrentDomainBaseDirectory);

            var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

            Assert.NotEmpty(result);
        }
        
        [Fact]
        public void Create_DoesNotCreateHypothesis_WhenLessThan100SnapshotsExistForTypeAndSubType()
        {
            var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
                    .ToUniversalTime();
            var fileName = nowDateTime.ToString(DateTimeFormat) + ".json";
            var snapshots = new List<List<string>>();
            for(int i = 0; i < 99; i++)
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
            var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryGetter(
                    CurrentDomainBaseDirectory);

            var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

            Assert.Empty(result);
        }

        [Fact]
        public void Create_AddsDataPointStrings_WhenTheyAreAtOrAboveThe10thPercentileForNumberOfSnapshotsTheyAppearIn()
        {
            var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
                    .ToUniversalTime();
            var fileName = nowDateTime.ToString(DateTimeFormat) + ".json";
            var testDataPointString = "testDataPointString";
            var snapshots = new List<List<string>>();
            for(int i = 0; i < 100; i++)
            {
                snapshots.Add(new List<string>() { "baseDataPointString" });
            }
            for(int i = 0; i < 10; i++)
            {
                snapshots[i].Add(testDataPointString);
            }
            var fileDataJson = JsonSerializer.Serialize(snapshots);
            var mockFileData = new MockFileData(fileDataJson);
            mockFileData.CreationTime = nowDateTime;
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { CreateDirectoryPath() + fileName, mockFileData }
            });
            var stubClock = new StubClock(nowDateTime);
            var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryGetter(
                    CurrentDomainBaseDirectory);

            var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

            Assert.True(result.Any(h => h.DataPointStrings.Contains(testDataPointString)));
        }
        
        [Fact]
        public void Create_FiltersOutSnapshotDataPointStrings_WhenTheyAreBelowThe10thPercentileForNumberOfSnapshotsTheyAppearIn()
        {
            var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
                    .ToUniversalTime();
            var fileName = nowDateTime.ToString(DateTimeFormat) + ".json";
            var invalidDataPointString = "invalidDataPointString";
            var snapshots = new List<List<string>>();
            for(int i = 0; i < 100; i++)
            {
                snapshots.Add(new List<string>() { "baseDataPointString" });
            }
            for(int i = 0; i < 9; i++)
            {
                snapshots[i].Add(invalidDataPointString);
            }
            var fileDataJson = JsonSerializer.Serialize(snapshots);
            var mockFileData = new MockFileData(fileDataJson);
            mockFileData.CreationTime = nowDateTime;
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { CreateDirectoryPath() + fileName, mockFileData }
            });
            var stubClock = new StubClock(nowDateTime);
            var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryGetter(
                    CurrentDomainBaseDirectory);

            var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

            Assert.False(result.Any(h => h.DataPointStrings.Contains(invalidDataPointString)));
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
            for(int i = 0; i < 100; i++)
            {
                snapshots.Add(new List<string>() { testDataPointString1 });
            }
            for(int i = 0; i < 75; i++)
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
            var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryGetter(
                    CurrentDomainBaseDirectory);

            var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);
            
            var dataPointStrings = new HashSet<string>()
            {
                testDataPointString1,
                testDataPointString2
            };
            Assert.True(result.Any(h => h.DataPointStrings.SetEquals(dataPointStrings)));
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
            for(int i = 0; i < 100; i++)
            {
                snapshots.Add(new List<string>() { testDataPointString1 });
            }
            for(int i = 0; i < 95; i++)
            {
                snapshots[i].Add(testDataPointString2);
            }
            for(int i = 0; i < 85; i++)
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
            var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryGetter(
                    CurrentDomainBaseDirectory);

            var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

            var hashSet1 = new HashSet<string>() { testDataPointString1 };
            var hashSet2 = new HashSet<string>() { testDataPointString1, testDataPointString2 };
            var hashSet3 = new HashSet<string>() { testDataPointString1, testDataPointString2,
                    testDataPointString3 };
            Assert.True(result.Any(h => h.DataPointStrings.SetEquals(hashSet1)));
            Assert.True(result.Any(h => h.DataPointStrings.SetEquals(hashSet2)));
            Assert.True(result.Any(h => h.DataPointStrings.SetEquals(hashSet3)));
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
            for(int i = 0; i < 100; i++)
            {
                snapshots.Add(new List<string>() { testDataPointString1 });
            }
            for(int i = 0; i < 95; i++)
            {
                snapshots[i].Add(testDataPointString2);
            }
            for(int i = 0; i < 85; i++)
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
            var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryGetter(
                    CurrentDomainBaseDirectory);

            var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);
            
            var invalidHashSet1 = new HashSet<string>() { testDataPointString2 };
            var invalidHashSet2 = new HashSet<string>() { testDataPointString3 };
            var invalidHashSet3 = new HashSet<string>() { testDataPointString1, testDataPointString3 };
            Assert.False(result.Any(h => h.DataPointStrings.SetEquals(invalidHashSet1)));
            Assert.False(result.Any(h => h.DataPointStrings.SetEquals(invalidHashSet2)));
            Assert.False(result.Any(h => h.DataPointStrings.SetEquals(invalidHashSet3)));
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
            for(int i = 0; i < 100; i++)
            {
                snapshots.Add(new List<string>() { testDataPointString1 });
            }
            for(int i = 0; i < 95; i++)
            {
                snapshots[i].Add(testDataPointString2);
            }
            for(int i = 0; i < 90; i++)
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
            var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryGetter(
                    CurrentDomainBaseDirectory);

            var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

            var validHashSet = new HashSet<string>() { testDataPointString1, testDataPointString2 };
            var invalidHashSet = new HashSet<string>() { testDataPointString2, testDataPointString3 };
            Assert.True(result.Any(h => h.DataPointStrings.SetEquals(validHashSet)));
            Assert.False(result.Any(h => h.DataPointStrings.SetEquals(invalidHashSet)));
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
            for(int i = 0; i < 100; i++)
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
            var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryGetter(
                    CurrentDomainBaseDirectory);

            var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

            Assert.False(result.Any(h => h.DataPointStrings.Contains(testDataPointString1)));
            Assert.True(result.Any(h => h.DataPointStrings.Contains(testDataPointString2)));
        }

        [Fact]
        public void Create_GetsAndAssignsCategoryTypeValueFromFilePath_WhenCalled()
        {
            var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
                    .ToUniversalTime();
            var fileName = nowDateTime.ToString(DateTimeFormat) + ".json";
            var testDataPointString = "testDataPointString";
            var snapshots = new List<List<string>>();
            for(int i = 0; i < 100; i++)
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
            var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryGetter(
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
            for(int i = 0; i < 100; i++)
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
            var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryGetter(
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
            for(int i = 0; i < 100; i++)
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
            var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryGetter(
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
            for(int i = 0; i < 100; i++)
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
            var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryGetter(
                    CurrentDomainBaseDirectory);

            var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

            Assert.Equal(SnapshotId, result[0].SnapshotId);
        }

        [Fact]
        public void Create_FiltersOutDataPointStrings_WhenTheyAppearIn80PercentOrMoreOfACategorysSnapshotIds()
        {
            var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
                    .ToUniversalTime();
            var fileName1 = nowDateTime.AddSeconds(-1).ToString(DateTimeFormat) + ".json";
            var fileName2 = nowDateTime.AddSeconds(-2).ToString(DateTimeFormat) + ".json";
            var fileName3 = nowDateTime.AddSeconds(-3).ToString(DateTimeFormat) + ".json";
            var fileName4 = nowDateTime.AddSeconds(-4).ToString(DateTimeFormat) + ".json";
            var fileName5 = nowDateTime.AddSeconds(-5).ToString(DateTimeFormat) + ".json";
            var fileName6 = nowDateTime.AddSeconds(-6).ToString(DateTimeFormat) + ".json";
            var fileName7 = nowDateTime.AddSeconds(-7).ToString(DateTimeFormat) + ".json";
            var fileName8 = nowDateTime.AddSeconds(-8).ToString(DateTimeFormat) + ".json";
            var fileName9 = nowDateTime.AddSeconds(-9).ToString(DateTimeFormat) + ".json";
            var fileName10 = nowDateTime.AddSeconds(-10).ToString(DateTimeFormat) + ".json";
            var invalidDataPointString = "invalidDataPointString";
            var snapshots1 = new List<List<string>>();
            var snapshots2 = new List<List<string>>();
            var snapshots3 = new List<List<string>>();
            var snapshots4 = new List<List<string>>();
            var snapshots5 = new List<List<string>>();
            var snapshots6 = new List<List<string>>();
            var snapshots7 = new List<List<string>>();
            var snapshots8 = new List<List<string>>();
            var snapshots9 = new List<List<string>>();
            var snapshots10 = new List<List<string>>();
            for(int i = 0; i < 100; i++)
            {
                snapshots1.Add(new List<string>() { "testDataPointString1", invalidDataPointString });
                snapshots2.Add(new List<string>() { "testDataPointString2", invalidDataPointString });
                snapshots3.Add(new List<string>() { "testDataPointString3", invalidDataPointString });
                snapshots4.Add(new List<string>() { "testDataPointString4", invalidDataPointString });
                snapshots5.Add(new List<string>() { "testDataPointString5", invalidDataPointString });
                snapshots6.Add(new List<string>() { "testDataPointString6", invalidDataPointString });
                snapshots7.Add(new List<string>() { "testDataPointString7", invalidDataPointString });
                snapshots8.Add(new List<string>() { "testDataPointString8", invalidDataPointString });
                snapshots9.Add(new List<string>() { "testDataPointString9", invalidDataPointString });
                snapshots10.Add(new List<string>() { "testDataPointString10", invalidDataPointString });
            }
            var mockFileData1 = new MockFileData(JsonSerializer.Serialize(snapshots1));
            var mockFileData2 = new MockFileData(JsonSerializer.Serialize(snapshots2));
            var mockFileData3 = new MockFileData(JsonSerializer.Serialize(snapshots3));
            var mockFileData4 = new MockFileData(JsonSerializer.Serialize(snapshots4));
            var mockFileData5 = new MockFileData(JsonSerializer.Serialize(snapshots5));
            var mockFileData6 = new MockFileData(JsonSerializer.Serialize(snapshots6));
            var mockFileData7 = new MockFileData(JsonSerializer.Serialize(snapshots7));
            var mockFileData8 = new MockFileData(JsonSerializer.Serialize(snapshots8));
            var mockFileData9 = new MockFileData(JsonSerializer.Serialize(snapshots9));
            var mockFileData10 = new MockFileData(JsonSerializer.Serialize(snapshots10));
            mockFileData1.CreationTime = nowDateTime;
            mockFileData2.CreationTime = nowDateTime;
            mockFileData3.CreationTime = nowDateTime;
            mockFileData4.CreationTime = nowDateTime;
            mockFileData5.CreationTime = nowDateTime;
            mockFileData6.CreationTime = nowDateTime;
            mockFileData7.CreationTime = nowDateTime;
            mockFileData8.CreationTime = nowDateTime;
            mockFileData9.CreationTime = nowDateTime;
            mockFileData10.CreationTime = nowDateTime;
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { CreateDirectoryPath("1") + fileName1, mockFileData1 },
                { CreateDirectoryPath("2") + fileName2, mockFileData2 },
                { CreateDirectoryPath("3") + fileName3, mockFileData3 },
                { CreateDirectoryPath("4") + fileName4, mockFileData4 },
                { CreateDirectoryPath("5") + fileName5, mockFileData5 },
                { CreateDirectoryPath("6") + fileName6, mockFileData6 },
                { CreateDirectoryPath("7") + fileName7, mockFileData7 },
                { CreateDirectoryPath("8") + fileName8, mockFileData8 },
                { CreateDirectoryPath("9") + fileName9, mockFileData9 },
                { CreateDirectoryPath("10") + fileName10, mockFileData10 }
            });
            var stubClock = new StubClock(nowDateTime);
            var stubCurrentDomainBaseGetter = new StubCurrentDomainBaseDirectoryGetter(
                    CurrentDomainBaseDirectory);

            var result = HypothesesCreator.Create(mockFileSystem, stubClock, stubCurrentDomainBaseGetter);

            Assert.False(result.Any(h => h.DataPointStrings.Contains(invalidDataPointString)));
            Assert.NotEmpty(result);
        }
    }
}
