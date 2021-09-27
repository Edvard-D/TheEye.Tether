using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using TheEyeTether.Types;
using TheEyeTether.UnitTests.Stubs;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class SnapshotDeleterTests
    {
        private const string DateTimeFormat = "yyyy_MM_dd__HH_mm_ss";
        private const string NowDateTimeString = "2021_09_23__12_00_00";


        [Fact]
        public void DeleteOutdatedFiles_ThrowsInvalidOperationException_WhenPassedNullDirectoryPath()
        {
            var keepLookbackDays = 1;
            var mockFileSystem = new MockFileSystem();

            try
            {
                SnapshotDeleter.DeleteOutdatedFiles(null, keepLookbackDays, mockFileSystem);
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
        public void DeleteOutdatedFiles_ThrowsInvalidOperationException_WhenPassedNegativeOrZeroLookbackDays(
                int keepLookbackDays)
        {
            var directoryPath = @"C:\";
            var mockFileSystem = new MockFileSystem();

            try
            {
                SnapshotDeleter.DeleteOutdatedFiles(directoryPath, keepLookbackDays, mockFileSystem);
                Assert.True(false);
            }
            catch(System.Exception ex)
            {
                Assert.IsType<System.InvalidOperationException>(ex);
            }
        }

        [Fact]
        public void DeleteOutdatedFiles_DoesNotDeleteFilesWithinKeepLookbackDays_WhenCalled()
        {
            var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null);
            var keepLookbackDays = 1;
            var creationDateTime = nowDateTime.AddDays(-keepLookbackDays);
            var directoryPath = @"C:\";
            var fileName = creationDateTime.ToString(DateTimeFormat) + ".json";
            var mockFileData = new MockFileData(string.Empty);
            mockFileData.CreationTime = creationDateTime;
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { directoryPath + fileName, mockFileData }
            });

            SnapshotDeleter.DeleteOutdatedFiles(directoryPath, keepLookbackDays, mockFileSystem);

            Assert.Contains(directoryPath + fileName, mockFileSystem.AllFiles);
        }
    }
}
