using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using TheEyeTether.Types;
using TheEyeTether.UnitTests.Stubs;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class SnapshotsLoaderTests
    {
        private const string DateTimeFormat = "yyyy_MM_dd__HH_mm_ss";
        private const string NowDateTimeString = "2021_09_23__12_00_00";


        [Fact]
        public void Load_ReturnsListOfListsOfStrings_WhenCalled()
        {
            var directoryPath = @"C:\TestDirectory\";
            var fileName = "2021_09_23__12_00_00.json";
            var snapshotData = new List<List<string>>()
            {
                new List<string>() { "test1" }
            };
            var jsonText = JsonSerializer.Serialize(snapshotData);
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { directoryPath + fileName, new MockFileData(jsonText) }
            });
            var stubClock = new StubClock(System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat,
                    null));

            var result = SnapshotsLoader.Load(directoryPath, 1, mockFileSystem, stubClock);

            Assert.IsType<List<List<string>>>(result);
        }

        [Fact]
        public void Load_ThrowsInvalidOperationException_WhenPassedNullDirectoryPath()
        {
            var mockFileSystem = new MockFileSystem();
            var stubClock = new StubClock(System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat,
                    null));

            try
            {
                var result = SnapshotsLoader.Load(null, 1, mockFileSystem, stubClock);
                Assert.True(false);
            }
            catch(System.Exception ex)
            {
                Assert.IsType<System.InvalidOperationException>(ex);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Load_ThrowsInvalidOperationException_WhenPassedNegativeOrZeroLookbackDays(int lookbackDays)
        {
            var mockFileSystem = new MockFileSystem();
            var stubClock = new StubClock(System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat,
                    null));

            try
            {
                var result = SnapshotsLoader.Load(null, lookbackDays, mockFileSystem, stubClock);
                Assert.True(false);
            }
            catch(System.Exception ex)
            {
                Assert.IsType<System.InvalidOperationException>(ex);
            }
        }

        [Fact]
        public void Load_ReturnsSnapshotsInFilesWithinDirectory_WhenCalled()
        {
            var directoryPath = @"C:\TestDirectory\";
            var fileName1 = "2021_09_23__12_00_00.json";
            var fileName2 = "2021_09_24__12_00_00.json";
            var snapshotData1 = new List<List<string>>()
            {
                new List<string>() { "test1" }
            };
            var snapshotData2 = new List<List<string>>()
            {
                new List<string>() { "test1", "test2", "test3" }
            };
            var jsonText1 = JsonSerializer.Serialize(snapshotData1);
            var jsonText2 = JsonSerializer.Serialize(snapshotData2);
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { directoryPath + fileName1, new MockFileData(jsonText1) },
                { directoryPath + fileName2, new MockFileData(jsonText2) }
            });
            var stubClock = new StubClock(System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat,
                    null));

            var result = SnapshotsLoader.Load(directoryPath, 1, mockFileSystem, stubClock);

            var expectedSnapshotCount = snapshotData1.Count + snapshotData2.Count;
            Assert.Equal(expectedSnapshotCount, result.Count);
        }

        [Fact]
        public void Load_DoesNotThrowException_WhenDirectoryContainsMisformattedFile()
        {
            var directoryPath = @"C:\TestDirectory\";
            var fileName = "2021_09_23__12_00_00.json";
            var snapshotData = new List<string>() { "test" };
            var jsonText = JsonSerializer.Serialize(snapshotData);
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { directoryPath + fileName, new MockFileData(jsonText) }
            });
            var stubClock = new StubClock(System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat,
                    null));

            var result = SnapshotsLoader.Load(directoryPath, 1, mockFileSystem, stubClock);
            
            Assert.True(true);
        }

        [Fact]
        public void Load_ReturnsSnapshotsInFilesWithinLookbackRange_WhenCalled()
        {
            var directoryPath = @"C:\TestDirectory\";
            var lookbackDays = 1;
            var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null);
            var fileName1 = NowDateTimeString + ".json";
            var fileName2 = nowDateTime.AddDays(-lookbackDays).ToString(DateTimeFormat) + ".json";
            var fileName3 = nowDateTime.AddDays(-lookbackDays - 1).ToString(DateTimeFormat) + ".json";
            var snapshotData = new List<List<string>>()
            {
                new List<string>() { "test" }
            };
            var jsonText1 = JsonSerializer.Serialize(snapshotData);
            var jsonText2 = JsonSerializer.Serialize(snapshotData);
            var jsonText3 = JsonSerializer.Serialize(snapshotData);
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { directoryPath + fileName1, new MockFileData(jsonText1) },
                { directoryPath + fileName2, new MockFileData(jsonText2) },
                { directoryPath + fileName3, new MockFileData(jsonText3) }
            });
            var stubClock = new StubClock(nowDateTime);

            var result = SnapshotsLoader.Load(directoryPath, lookbackDays, mockFileSystem, stubClock);

            var expectedSnapshotCount = snapshotData.Count * 2;
            Assert.Equal(expectedSnapshotCount, result.Count);
        }
    }
}
