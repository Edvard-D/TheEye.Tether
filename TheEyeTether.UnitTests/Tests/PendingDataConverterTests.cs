using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Runtime.InteropServices;
using TheEyeTether.Types;
using TheEyeTether.UnitTests.Stubs;
using Xunit;


namespace TheEyeTether.UnitTests
{
    public class PendingDataConverterTests
    {
        [Fact]
        public void Execute_CreatesFiles_WhenThereIsPendingData()
        {
            var programPath = @"C:\WorldOfWarcraft\_retail_\Wow.exe";
            var pendingDataFilePath = @"C:\WorldOfWarcraft\_retail_\WTF\Account\AccountName\ServerName\CharacterName\SavedVariables\TheEyeRecorder.lua";
            var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) },
                { pendingDataFilePath, new MockFileData(string.Empty) }
            });
            var drivesGetterStub = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformCheckerStub = new OSPlatformCheckerStub(OSPlatform.Windows);

            PendingDataConverter.Execute(fileSystemMock, drivesGetterStub, osPlatformCheckerStub);

            var allFiles = fileSystemMock.AllFiles as string[];
            Assert.Equal(3, allFiles.Length);
        }
        
        [Fact]
        public void Execute_DoesNotThrowError_WhenProgramCannotBeFound()
        {
            var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>() {});
            var drivesGetterStub = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformCheckerStub = new OSPlatformCheckerStub(OSPlatform.Windows);

            PendingDataConverter.Execute(fileSystemMock, drivesGetterStub, osPlatformCheckerStub);
        }
    }
}
