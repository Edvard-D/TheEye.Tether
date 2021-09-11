using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Runtime.InteropServices;
using TheEyeTether.Types;
using TheEyeTether.UnitTests.Stubs;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
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
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

            var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
                    mockFileSystem, stubDrivesGetter, stubOSPlatformChecker);
            
            Assert.IsType<string>(result);
        }

        [Fact]
        public void LocateProgramPath_ReturnsNull_WhenProgramDoesNotExist()
        {
            var programName = "test.exe";
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {});
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

            var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
                    mockFileSystem, stubDrivesGetter, stubOSPlatformChecker);

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
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

            var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
                    mockFileSystem, stubDrivesGetter, stubOSPlatformChecker);

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
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

            var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
                    mockFileSystem, stubDrivesGetter, stubOSPlatformChecker);

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
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

            var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
                    mockFileSystem, stubDrivesGetter, stubOSPlatformChecker);

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
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

            var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
                    mockFileSystem, stubDrivesGetter, stubOSPlatformChecker);

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
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

            var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
                    mockFileSystem, stubDrivesGetter, stubOSPlatformChecker);

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
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.OSX);

            var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
                    mockFileSystem, stubDrivesGetter, stubOSPlatformChecker);

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
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

            var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
                    mockFileSystem, stubDrivesGetter, stubOSPlatformChecker);

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
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.OSX);

            var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
                    mockFileSystem, stubDrivesGetter, stubOSPlatformChecker);

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
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

            var result = ProgramPathLocater.LocateProgramPath(programName, string.Empty,
                    mockFileSystem, stubDrivesGetter, stubOSPlatformChecker, correctProgramDirectory);

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
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

            var result = ProgramPathLocater.LocateProgramPath(programName, correctProgramDirectory,
                    mockFileSystem, stubDrivesGetter, stubOSPlatformChecker);

            Assert.Equal(result, correctProgramPath);
        }
    }
}
