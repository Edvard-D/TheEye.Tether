using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Moq;
using TheEye.Tether.UnitTests.Stubs;
using TheEye.Tether.Utilities.General;
using Xunit;

namespace TheEye.Tether.UnitTests.Tests.Utilities.General
{
	public class FilePathAggregatorTests
	{
		[Fact]
		public void LocateFiles_ReturnsListOfStrings_WhenCalled()
		{
			var searchDirectory = @"C:\";
			var requiredDirectories = string.Empty;
			var fileName = string.Empty;
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
			mockFileSystem.Directory.CreateDirectory(@"C:\");

			var results = FilePathAggregator.Aggregate(searchDirectory, requiredDirectories,
					fileName, mockFileSystem);

			Assert.IsType<List<string>>(results);
		}

		[Fact]
		public void LocateFiles_ReturnsFilePaths_WhenFileNameMatches()
		{
			var searchDirectory = @"C:\";
			var requiredDirectories = string.Empty;
			var fileName = "correctFile.txt";
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ searchDirectory + fileName, new MockFileData(string.Empty) }
			});

			var results = FilePathAggregator.Aggregate(searchDirectory, requiredDirectories,
					fileName, mockFileSystem);

			Assert.Single(results);
		}

		[Fact]
		public void LocateFiles_DoesNotReturnFilePaths_WhenFileNameDoesNotMatch()
		{
			var searchDirectory = @"C:\";
			var requiredDirectories = string.Empty;
			var incorrectFileName = "incorrectFile.txt";
			var correctFileName = "correctFile.txt";
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ searchDirectory + incorrectFileName, new MockFileData(string.Empty) }
			});

			var results = FilePathAggregator.Aggregate(searchDirectory, requiredDirectories,
					correctFileName, mockFileSystem);

			Assert.Empty(results);
		}

		[Fact]
		public void LocateProgramPath_EnsuresPathContainsRequiredDirectories_WhenCalled()
		{
			var searchDirectory = @"C:\";
			var fileName = "test.exe";
			var correctProgramDirectory = @"Users\Test1\";
			var correctProgramPath = searchDirectory + correctProgramDirectory + fileName;
			var incorrectProgramPath = searchDirectory + @"Users\Test2\" + fileName;
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ incorrectProgramPath, new MockFileData(string.Empty) },
				{ correctProgramPath, new MockFileData(string.Empty) }
			});

			var results = FilePathAggregator.Aggregate(searchDirectory, correctProgramDirectory,
					fileName, mockFileSystem);

			Assert.Contains(results, r => r == correctProgramPath);
		}

		[Fact]
		public void LocateProgramPath_DoesNotThrowUnauthorizedAccessException_WhenAccessingDirectoriesNotAuthorizedToAccess()
		{
			var searchDirectory = @"C:\";
			var fileName = "test.exe";
			var correctProgramDirectory = @"Users\Test1\";
			var correctProgramPath = searchDirectory + correctProgramDirectory + fileName;
			var mockFileSystem = new Mock<IFileSystem>();
			mockFileSystem.Setup(x => x.File.WriteAllText(It.IsAny<string>(), It.IsAny<string>()))
					.Throws(new System.UnauthorizedAccessException());
			mockFileSystem.Setup(x => x.DirectoryInfo.FromDirectoryName(It.IsAny<string>()))
					.Returns(new StubDirectoryInfo(searchDirectory));

			var exception = Record.Exception(() => FilePathAggregator.Aggregate(searchDirectory,
					correctProgramDirectory, fileName, mockFileSystem.Object));
			
			Assert.Null(exception);
		}

		[Fact]
		public void LocateProgramPath_DoesNotThrowIOException_WhenAccessingDirectoriesThatArentReady()
		{
			var searchDirectory = @"C:\";
			var fileName = "test.exe";
			var correctProgramDirectory = @"Users\Test1\";
			var correctProgramPath = searchDirectory + correctProgramDirectory + fileName;
			var mockFileSystem = new Mock<IFileSystem>();
			mockFileSystem.Setup(x => x.File.WriteAllText(It.IsAny<string>(), It.IsAny<string>()))
					.Throws(new System.IO.IOException());
			mockFileSystem.Setup(x => x.DirectoryInfo.FromDirectoryName(It.IsAny<string>()))
					.Returns(new StubDirectoryInfo(@"C:\"));

			var exception = Record.Exception(() => FilePathAggregator.Aggregate(searchDirectory,
					correctProgramDirectory, fileName, mockFileSystem.Object));
			
			Assert.Null(exception);
		}
	}
}
