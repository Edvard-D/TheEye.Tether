using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Runtime.InteropServices;
using Moq;
using TheEye.Tether.UnitTests.Stubs;
using TheEye.Tether.Utilities.General;
using Xunit;

namespace TheEye.Tether.UnitTests.Tests.Utilities.General
{
	public class ProgramPathLocaterTests
	{
		[Fact]
		public void LocateProgramPath_ReturnsString_WhenProgramExists()
		{
			var programName = "test.exe";
			var programPath = @"C:\test.exe";
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ programPath, new MockFileData(string.Empty) }
			});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

			var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
					mockFileSystem, stubDrivesProvider, stubOSPlatformChecker);
			
			Assert.IsType<string>(result);
		}

		[Fact]
		public void LocateProgramPath_ReturnsNull_WhenProgramDoesNotExist()
		{
			var programName = "test.exe";
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

			var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
					mockFileSystem, stubDrivesProvider, stubOSPlatformChecker);

			Assert.Null(result);
		}

		[Fact]
		public void LocateProgramPath_ReturnsProgramPath_WhenPassedValidProgramName()
		{
			var programName = "test.exe";
			var programPath = @"C:\test.exe";
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ programPath, new MockFileData(string.Empty) }
			});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

			var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
					mockFileSystem, stubDrivesProvider, stubOSPlatformChecker);

			Assert.Equal(programPath, result);
		}

		[Fact]
		public void LocateProgramPath_ReturnsPreviouslySavedPath_WhenPreviouslySavedPathStillExists()
		{
			var programName = "test.exe";
			var correctProgramPath = @"C:\test1.exe";
			var incorrectProgramPath = @"C:\test2.exe";
			ProgramPathLocater.SavedProgramPathPairs[programName] = correctProgramPath;
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ incorrectProgramPath, new MockFileData(string.Empty) },
				{ correctProgramPath, new MockFileData(string.Empty) }
			});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

			var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
					mockFileSystem, stubDrivesProvider, stubOSPlatformChecker);

			Assert.Equal(correctProgramPath, result);
		}

		[Fact]
		public void LocateProgramPath_ReturnsNewPath_WhenPreviouslySavedPathDoesNotExist()
		{
			var programName = "test.exe";
			var correctProgramPath = @"C:\Users\Test\test.exe";
			var incorrectProgramPath = @"C:\test.exe";
			ProgramPathLocater.SavedProgramPathPairs[programName] = incorrectProgramPath;
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ correctProgramPath, new MockFileData(string.Empty) }
			});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

			var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
					mockFileSystem, stubDrivesProvider, stubOSPlatformChecker);

			Assert.Equal(correctProgramPath, result);
		}

		[Fact]
		public void LocateProgramPath_SavesProgramPathPair_WhenLocated()
		{
			var programName = "test.exe";
			var programPath = @"C:\test.exe";
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ programPath, new MockFileData(string.Empty) }
			});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

			var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
					mockFileSystem, stubDrivesProvider, stubOSPlatformChecker);

			Assert.Contains(programName, ProgramPathLocater.SavedProgramPathPairs.Keys);
		}

		[Fact]
		public void LocateProgramPath_AddsExeToProgramName_WhenCalledOnWindowsAndProgramNameDoesNotHaveExe()
		{
			var programName = "test";
			var programPath = @"C:\test.exe";
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ programPath, new MockFileData(string.Empty) }
			});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

			var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
					mockFileSystem, stubDrivesProvider, stubOSPlatformChecker);

			Assert.Equal(programPath, result);
		}

		[Fact]
		public void LocateProgramPath_AddsAppToProgramName_WhenCalledOnMacOSAndProgramNameDoesNotHaveApp()
		{
			var programName = "test";
			var programPath = @"C:\test.app";
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ programPath, new MockFileData(string.Empty) }
			});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.OSX);

			var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
					mockFileSystem, stubDrivesProvider, stubOSPlatformChecker);

			Assert.Equal(programPath, result);
		}

		[Fact]
		public void LocateProgramPath_DoesNotAddExeToProgramName_WhenCalledOnWindowsAndProgramNameDoesHaveExe()
		{
			var programName = "test.exe";
			var programPath = @"C:\test.exe";
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ programPath, new MockFileData(string.Empty) }
			});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

			var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
					mockFileSystem, stubDrivesProvider, stubOSPlatformChecker);

			Assert.Equal(programPath, result);
		}

		[Fact]
		public void LocateProgramPath_DoesNotAddAppToProgramName_WhenCalledOnMacOSAndProgramNameDoesHaveApp()
		{
			var programName = "test.app";
			var programPath = @"C:\test.app";
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ programPath, new MockFileData(string.Empty) }
			});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.OSX);

			var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
					mockFileSystem, stubDrivesProvider, stubOSPlatformChecker);

			Assert.Equal(programPath, result);
		}

		[Fact]
		public void LocateProgramPath_ChecksDefaultPathFirst_WhenDefaultPathIsPassed()
		{
			var programName = "test.exe";
			var correctProgramDirectory = @"C:\Users\Test1\";
			var correctProgramPath = correctProgramDirectory + programName;
			var incorrectProgramPath = @"C:\Users\Test2\test.exe";
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ incorrectProgramPath, new MockFileData(string.Empty) },
				{ correctProgramPath, new MockFileData(string.Empty) }
			});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

			var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
					mockFileSystem, stubDrivesProvider, stubOSPlatformChecker, correctProgramDirectory);

			Assert.Equal(result, correctProgramPath);
		}

		[Fact]
		public void LocateProgramPath_EnsuresPathContainsRequiredDirectories_WhenCalled()
		{
			var programName = "test.exe";
			var correctProgramDirectory = @"Users\Test1\";
			var correctProgramPath = @"C:\" + correctProgramDirectory + programName;
			var incorrectProgramPath = @"C:\Users\Test2\test.exe";
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ incorrectProgramPath, new MockFileData(string.Empty) },
				{ correctProgramPath, new MockFileData(string.Empty) }
			});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

			var result = ProgramPathLocater.LocateProgramPath(programName, correctProgramDirectory,
					mockFileSystem, stubDrivesProvider, stubOSPlatformChecker);

			Assert.Equal(result, correctProgramPath);
		}

		[Fact]
		public void LocateProgramPath_DoesNotThrowUnauthorizedAccessException_WhenSearchingDirectoriesNotAuthorizedToAccess()
		{
			var programName = "test.exe";
			var correctProgramDirectory = @"Users\Test1\";
			var correctProgramPath = @"C:\" + correctProgramDirectory + programName;
			var mockFileSystem = new Mock<IFileSystem>();
			mockFileSystem.Setup(x => x.File.WriteAllText(It.IsAny<string>(), It.IsAny<string>()))
					.Throws(new System.UnauthorizedAccessException());
			mockFileSystem.Setup(x => x.DirectoryInfo.FromDirectoryName(It.IsAny<string>()))
					.Returns(new StubDirectoryInfo(@"C:\"));
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

			var exception = Record.Exception(() => ProgramPathLocater.LocateProgramPath(programName,
					correctProgramDirectory, mockFileSystem.Object, stubDrivesProvider,
					stubOSPlatformChecker));
			
			Assert.Null(exception);
		}
	}
}
