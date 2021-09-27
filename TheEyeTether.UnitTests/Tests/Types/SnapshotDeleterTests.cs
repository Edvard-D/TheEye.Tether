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
            try
            {
                SnapshotDeleter.DeleteOutdatedFiles(null);
                Assert.True(false);
            }
            catch(System.Exception ex)
            {
                Assert.IsType<System.InvalidOperationException>(ex);
            }
        }
    }
}
