using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class SnapshotsLoaderTests
    {
        [Fact]
        public void Load_ReturnsListOfListsOfStrings_WhenCalled()
        {
            var directoryPath = @"C:\TestDirectory\";
            var snapshotData = new List<List<string>>()
            {
                new List<string>() { "test1" }
            };
            var jsonText = JsonSerializer.Serialize(snapshotData);
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { directoryPath + fileName, new MockFileData(jsonText) }
            });

            var result = SnapshotsLoader.Load(directoryPath, 1, mockFileSystem);

            Assert.IsType<List<List<string>>>(result);
        }

        [Fact]
        public void Load_ThrowsInvalidOperationException_WhenPassedNullDirectoryPath()
        {
            var mockFileSystem = new MockFileSystem();

            try
            {
                var result = SnapshotsLoader.Load(null, 1, mockFileSystem);
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

            try
            {
                var result = SnapshotsLoader.Load(null, lookbackDays, mockFileSystem);
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
            var fileName1 = "test1.json";
            var fileName2 = "test2.json";
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

            var result = SnapshotsLoader.Load(directoryPath, 1, mockFileSystem);

            var expectedSnapshotCount = snapshotData1.Count + snapshotData2.Count;
            Assert.Equal(expectedSnapshotCount, result.Count);
        }
    }
}
