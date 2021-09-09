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
    }
}
