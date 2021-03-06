using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using TheEyeTether.Utilities.General;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Utilities.General
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
        public void AggregateFilePaths_ReturnsFilePaths_WhenFileNameMatches()
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

        [Fact]
        public void AggregateFilePaths_DoesNotReturnFilePaths_WhenFileNameDoesNotMatch()
        {
            var correctFileName = "test1.txt";
            var incorrectFileName = "test2.txt";
            var searchDirectoryPath = @"C:Test\SearchDirectory\";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { searchDirectoryPath + incorrectFileName, new MockFileData(string.Empty) }
            });

            var results = FilePathAggregator.AggregateFilePaths(correctFileName, searchDirectoryPath,
                    fileSystem);

            Assert.Empty(results);
        }
    }
}
