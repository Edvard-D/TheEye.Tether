using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using TheEyeTether.Types;
using TheEyeTether.UnitTests.Stubs;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class HypothesesCreatorTests
    {
        private const string DateTimeFormat = "yyyy_MM_dd__HH_mm_ss";
        private const string NowDateTimeString = "2021_09_23__12_00_00";


        [Fact]
        public void Create_ReturnsListOfHypotheses_WhenCalled()
        {
            var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
                    .ToUniversalTime();
            var mockFileSystem = new MockFileSystem();
            var stubClock = new StubClock(nowDateTime);

            var result = HypothesesCreator.Create(mockFileSystem, stubClock);

            Assert.IsType<List<Hypothesis>>(result);
        }

        [Fact]
        public void Create_DeletesFile_WhenOlderThan7Days()
        {
            var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
                    .ToUniversalTime();
            var creationDateTime = nowDateTime.AddDays(-8);
            var directoryPath = @"C:\";
            var fileName = creationDateTime.ToString(DateTimeFormat) + ".json";
            var mockFileData = new MockFileData(string.Empty);
            mockFileData.CreationTime = creationDateTime;
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { directoryPath + fileName, mockFileData }
            });
            var stubClock = new StubClock(nowDateTime);

            var result = HypothesesCreator.Create(mockFileSystem, stubClock);

            Assert.DoesNotContain(directoryPath + fileName, mockFileSystem.AllFiles);
        }

        [Fact]
        public void Create_CreatesHypothesis_WhenGreaterThanEqualTo100SnapshotsExistForTypeAndSubType()
        {
            var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
                    .ToUniversalTime();
            var directoryPath = @"C:\";
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
                { directoryPath + fileName, mockFileData }
            });
            var stubClock = new StubClock(nowDateTime);

            var result = HypothesesCreator.Create(mockFileSystem, stubClock);

            Assert.NotEmpty(result);
        }
        
        [Fact]
        public void Create_DoesNotCreateHypothesis_WhenLessThan100SnapshotsExistForTypeAndSubType()
        {
            var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
                    .ToUniversalTime();
            var directoryPath = @"C:\";
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
                { directoryPath + fileName, mockFileData }
            });
            var stubClock = new StubClock(nowDateTime);

            var result = HypothesesCreator.Create(mockFileSystem, stubClock);

            Assert.Empty(result);
        }

        [Fact]
        public void Create_AddsDataPointStrings_WhenTheyAreAtOrAboveThe25thPercentileForNumberOfSnapshotsTheyAppearIn()
        {
            var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
                    .ToUniversalTime();
            var directoryPath = @"C:\";
            var fileName = nowDateTime.ToString(DateTimeFormat) + ".json";
            var testDataPointString = "testDataPointString";
            var snapshots = new List<List<string>>();
            for(int i = 0; i < 100; i++)
            {
                snapshots.Add(new List<string>() { "baseDataPointString" });
            }
            for(int i = 0; i < 25; i++)
            {
                snapshots[i].Add(testDataPointString);
            }
            var fileDataJson = JsonSerializer.Serialize(snapshots);
            var mockFileData = new MockFileData(fileDataJson);
            mockFileData.CreationTime = nowDateTime;
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { directoryPath + fileName, mockFileData }
            });
            var stubClock = new StubClock(nowDateTime);

            var result = HypothesesCreator.Create(mockFileSystem, stubClock);

            Assert.Contains(testDataPointString, result[0].DataPointStrings);
        }
        
        [Fact]
        public void Create_FiltersOutSnapshotDataPointStrings_WhenTheyAreBelowThe25thPercentileForNumberOfSnapshotsTheyAppearIn()
        {
            var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
                    .ToUniversalTime();
            var directoryPath = @"C:\";
            var fileName = nowDateTime.ToString(DateTimeFormat) + ".json";
            var invalidDataPointString = "invalidDataPointString";
            var snapshots = new List<List<string>>();
            for(int i = 0; i < 100; i++)
            {
                snapshots.Add(new List<string>() { "baseDataPointString" });
            }
            for(int i = 0; i < 24; i++)
            {
                snapshots[i].Add(invalidDataPointString);
            }
            var fileDataJson = JsonSerializer.Serialize(snapshots);
            var mockFileData = new MockFileData(fileDataJson);
            mockFileData.CreationTime = nowDateTime;
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { directoryPath + fileName, mockFileData }
            });
            var stubClock = new StubClock(nowDateTime);

            var result = HypothesesCreator.Create(mockFileSystem, stubClock);

            Assert.DoesNotContain(invalidDataPointString, result[0].DataPointStrings);
        }
    }
}
