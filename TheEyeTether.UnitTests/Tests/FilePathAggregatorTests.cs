using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests
{
    public class FilePathAggregatorTests
    {
        [Fact]
        public void AggregateFilePaths_ReturnsArrayOfStrings_WhenCalled()
        {
            var fileName = string.Empty;
            var searchDirectoryPath = string.Empty;
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());

            var results = FilePathAggregator.AggregateFilePaths(fileName, searchDirectoryPath, fileSystem);

            Assert.IsType<string[]>(results);
        }

        [Fact]
        public void AggregateFilePaths_ReturnsFilePathsWithMatchingFileName_WhenFilesExist()
        {
            var fileName = "test.txt";
            var searchDirectoryPath = @"C:\Test\SearchDirectory\";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { searchDirectoryPath + @"SubDirectory1\" + fileName, new MockFileData(string.Empty) },
                { searchDirectoryPath + @"SubDirectory2\" + fileName, new MockFileData(string.Empty) },
            });

            var results = FilePathAggregator.AggregateFilePaths(fileName, searchDirectoryPath, fileSystem);

            Assert.Equal(2, results.Length);
        }
    }
}
