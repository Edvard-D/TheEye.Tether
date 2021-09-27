using System.IO.Abstractions.TestingHelpers;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class SnapshotDeleterTests
    {
        [Fact]
        public void DeleteOutdatedFiles_ThrowsInvalidOperationException_WhenPassedNullDirectoryPath()
        {
            var keepLookbackDays = 1;

            try
            {
                SnapshotDeleter.DeleteOutdatedFiles(null, keepLookbackDays);
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

            try
            {
                SnapshotDeleter.DeleteOutdatedFiles(directoryPath, keepLookbackDays);
                Assert.True(false);
            }
            catch(System.Exception ex)
            {
                Assert.IsType<System.InvalidOperationException>(ex);
            }
        }
    }
}
