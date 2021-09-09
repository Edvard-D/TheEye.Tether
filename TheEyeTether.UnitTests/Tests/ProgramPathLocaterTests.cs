using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Runtime.InteropServices;
using TheEyeTether.Types;
using TheEyeTether.UnitTests.Stubs;
using Xunit;

namespace TheEyeTether.UnitTests
{
    public class ProgramPathLocaterTests
    {
        [Fact]
        public void LocateProgramPath_ReturnsString_WhenProgramExists()
        {
            var programName = "test.exe";
            var programPath = @"C:\test.exe";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { programPath, new MockFileData(string.Empty) }
            });
            var drivesGetter = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformChecker = new OSPlatformCheckerStub(OSPlatform.Windows);

            var result = ProgramPathLocater.LocateProgramPath(programName, fileSystem, drivesGetter,
                    osPlatformChecker);
            
            Assert.IsType<string>(result);
        }

        [Fact]
        public void LocateProgramPath_ReturnsNull_WhenProgramDoesNotExist()
        {
            var programName = "test.exe";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {});
            var drivesGetter = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformChecker = new OSPlatformCheckerStub(OSPlatform.Windows);

            var result = ProgramPathLocater.LocateProgramPath(programName, fileSystem, drivesGetter,
                    osPlatformChecker);

            Assert.Null(result);
        }

        [Fact]
        public void LocateProgramPath_ReturnsProgramPath_WhenPassedValidProgramName()
        {
            var programName = "test.exe";
            var programPath = @"C:\test.exe";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { programPath, new MockFileData(string.Empty) }
            });
            var drivesGetter = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformChecker = new OSPlatformCheckerStub(OSPlatform.Windows);

            var result = ProgramPathLocater.LocateProgramPath(programName, fileSystem, drivesGetter,
                    osPlatformChecker);

            Assert.Equal(programPath, result);
        }

        [Fact]
        public void LocateProgramPath_ReturnsPreviouslySavedPath_WhenPreviouslySavedPathStillExists()
        {
            var programName = "test.exe";
            var correctProgramPath = @"C:\test1.exe";
            var incorrectProgramPath = @"C:\test2.exe";
            ProgramPathLocater.SavedProgramPathPairs[programName] = correctProgramPath;
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { incorrectProgramPath, new MockFileData(string.Empty) },
                { correctProgramPath, new MockFileData(string.Empty) }
            });
            var drivesGetter = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformChecker = new OSPlatformCheckerStub(OSPlatform.Windows);

            var result = ProgramPathLocater.LocateProgramPath(programName, fileSystem, drivesGetter,
                    osPlatformChecker);

            Assert.Equal(correctProgramPath, result);
        }

        [Fact]
        public void LocateProgramPath_ReturnsNewPath_WhenPreviouslySavedPathDoesNotExist()
        {
            var programName = "test.exe";
            var correctProgramPath = @"C:\Users\Test\test.exe";
            var incorrectProgramPath = @"C:\test.exe";
            ProgramPathLocater.SavedProgramPathPairs[programName] = incorrectProgramPath;
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { correctProgramPath, new MockFileData(string.Empty) }
            });
            var drivesGetter = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformChecker = new OSPlatformCheckerStub(OSPlatform.Windows);

            var result = ProgramPathLocater.LocateProgramPath(programName, fileSystem, drivesGetter,
                    osPlatformChecker);

            Assert.Equal(correctProgramPath, result);
        }

        [Fact]
        public void LocateProgramPath_SavesProgramPathPair_WhenLocated()
        {
            var programName = "test.exe";
            var programPath = @"C:\test.exe";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { programPath, new MockFileData(string.Empty) }
            });
            var drivesGetter = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformChecker = new OSPlatformCheckerStub(OSPlatform.Windows);

            var result = ProgramPathLocater.LocateProgramPath(programName, fileSystem, drivesGetter,
                    osPlatformChecker);

            Assert.Contains(programName, ProgramPathLocater.SavedProgramPathPairs.Keys);
        }

        [Fact]
        public void LocateProgramPath_AddsExeToProgramName_WhenCalledOnWindowsAndProgramNameDoesNotHaveExe()
        {
            var programName = "test";
            var programPath = @"C:\test.exe";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { programPath, new MockFileData(string.Empty) }
            });
            var drivesGetter = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformChecker = new OSPlatformCheckerStub(OSPlatform.Windows);

            var result = ProgramPathLocater.LocateProgramPath(programName, fileSystem, drivesGetter,
                    osPlatformChecker);

            Assert.Equal(programPath, result);
        }

        [Fact]
        public void LocateProgramPath_AddsAppToProgramName_WhenCalledOnMacOSAndProgramNameDoesNotHaveApp()
        {
            var programName = "test";
            var programPath = @"C:\test.app";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { programPath, new MockFileData(string.Empty) }
            });
            var drivesGetter = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformChecker = new OSPlatformCheckerStub(OSPlatform.OSX);

            var result = ProgramPathLocater.LocateProgramPath(programName, fileSystem, drivesGetter,
                    osPlatformChecker);

            Assert.Equal(programPath, result);
        }

        [Fact]
        public void LocateProgramPath_DoesNotAddExeToProgramName_WhenCalledOnWindowsAndProgramNameDoesHaveExe()
        {
            var programName = "test.exe";
            var programPath = @"C:\test.exe";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { programPath, new MockFileData(string.Empty) }
            });
            var drivesGetter = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformChecker = new OSPlatformCheckerStub(OSPlatform.Windows);

            var result = ProgramPathLocater.LocateProgramPath(programName, fileSystem, drivesGetter,
                    osPlatformChecker);

            Assert.Equal(programPath, result);
        }

        [Fact]
        public void LocateProgramPath_DoesNotAddAppToProgramName_WhenCalledOnMacOSAndProgramNameDoesHaveApp()
        {
            var programName = "test.app";
            var programPath = @"C:\test.app";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { programPath, new MockFileData(string.Empty) }
            });
            var drivesGetter = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformChecker = new OSPlatformCheckerStub(OSPlatform.OSX);

            var result = ProgramPathLocater.LocateProgramPath(programName, fileSystem, drivesGetter,
                    osPlatformChecker);

            Assert.Equal(programPath, result);
        }

        [Fact]
        public void LocateProgramPath_ChecksDefaultPathFirst_WhenDefaultPathIsPassed()
        {
            var programName = "test.exe";
            var correctProgramDirectory = @"C:\Users\Test1\";
            var correctProgramPath = correctProgramDirectory + programName;
            var incorrectProgramPath = @"C:\Users\Test2\test.exe";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { incorrectProgramPath, new MockFileData(string.Empty) },
                { correctProgramPath, new MockFileData(string.Empty) }
            });
            var drivesGetter = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformChecker = new OSPlatformCheckerStub(OSPlatform.Windows);

            var result = ProgramPathLocater.LocateProgramPath(programName, fileSystem, drivesGetter,
                    osPlatformChecker, correctProgramDirectory);

            Assert.Equal(result, correctProgramPath);
        }
    }
}
