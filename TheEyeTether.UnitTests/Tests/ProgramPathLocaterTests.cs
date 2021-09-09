using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using TheEyeTether.Types;
using TheEyeTether.UnitTests.Stubs;
using Xunit;

namespace TheEyeTether.UnitTests
{
    public class ProgramPathLocaterTests
    {
        [Fact]
        public void LocateProgramPath_ReturnsString_WhenCalled()
        {
            var programName = string.Empty;
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {});
            var drivesGetter = new DrivesGetterStub(new List<string>());

            var result = ProgramPathLocater.LocateProgramPath(programName, fileSystem, drivesGetter);
            
            Assert.IsType<string>(result);
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

            var result = ProgramPathLocater.LocateProgramPath(programName, fileSystem, drivesGetter);

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

            var result = ProgramPathLocater.LocateProgramPath(programName, fileSystem, drivesGetter);

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

            var result = ProgramPathLocater.LocateProgramPath(programName, fileSystem, drivesGetter);

            Assert.Equal(correctProgramPath, result);
        }
    }
}
