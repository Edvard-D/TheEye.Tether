using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
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
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) },
                { pendingDataFilePath, new MockFileData(string.Empty) }
            });
            var drivesGetterStub = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformCheckerStub = new OSPlatformCheckerStub(OSPlatform.Windows);
            var currentDomainBaseDirectoryGetter = new CurrentDomainBaseDirectoryGetter(currentDomainBaseDirectory);

            PendingDataConverter.Execute(fileSystemMock, drivesGetterStub, osPlatformCheckerStub,
                    currentDomainBaseDirectoryGetter);

            var allFiles = fileSystemMock.AllFiles as string[];
            Assert.Equal(3, allFiles.Length);
        }
        
        [Fact]
        public void Execute_DoesNotThrowError_WhenProgramCannotBeFound()
        {
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>() {});
            var drivesGetterStub = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformCheckerStub = new OSPlatformCheckerStub(OSPlatform.Windows);
            var currentDomainBaseDirectoryGetter = new CurrentDomainBaseDirectoryGetter(currentDomainBaseDirectory);

            PendingDataConverter.Execute(fileSystemMock, drivesGetterStub, osPlatformCheckerStub,
                    currentDomainBaseDirectoryGetter);
        }

        [Fact]
        public void Execute_RemovesFileEndingFromProgramName_WhenCalledOnWindows()
        {
            var programPath = @"C:\WorldOfWarcraft\_retail_\Wow.exe";
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) }
            });
            var drivesGetterStub = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformCheckerStub = new OSPlatformCheckerStub(OSPlatform.Windows);
            var currentDomainBaseDirectoryGetter = new CurrentDomainBaseDirectoryGetter(currentDomainBaseDirectory);

            PendingDataConverter.Execute(fileSystemMock, drivesGetterStub, osPlatformCheckerStub,
                    currentDomainBaseDirectoryGetter);
        }

        [Fact]
        public void Execute_RemovesFileEndingFromProgramName_WhenCalledOnMacOS()
        {
            var programPath = @"C:\Applications\WorldOfWarcraft\_retail_\Wow.app";
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) }
            });
            var drivesGetterStub = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformCheckerStub = new OSPlatformCheckerStub(OSPlatform.OSX);
            var currentDomainBaseDirectoryGetter = new CurrentDomainBaseDirectoryGetter(currentDomainBaseDirectory);

            PendingDataConverter.Execute(fileSystemMock, drivesGetterStub, osPlatformCheckerStub,
                    currentDomainBaseDirectoryGetter);
        }
        
        [Fact]
        public void Execute_CreatesNewFileWithCurrentDomainBaseDirectoryInPath_WhenThereIsPendingData()
        {
            var programPath = @"C:\WorldOfWarcraft\_retail_\Wow.exe";
            var accountName = "AccountName";
            var serverName = "ServerName";
            var characterName = "CharacterName";
            var pendingDataFilePath = string.Format(@"C:\WorldOfWarcraft\_retail_\WTF\Account\{0}\{1}\{2}\SavedVariables\TheEyeRecorder.lua",
                    accountName, serverName, characterName);
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) },
                { pendingDataFilePath, new MockFileData(string.Empty) },
                { currentDomainBaseDirectory, new MockFileData(string.Empty) },
            });
            var drivesGetterStub = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformCheckerStub = new OSPlatformCheckerStub(OSPlatform.Windows);
            var currentDomainBaseDirectoryGetter = new CurrentDomainBaseDirectoryGetter(currentDomainBaseDirectory);

            PendingDataConverter.Execute(fileSystemMock, drivesGetterStub, osPlatformCheckerStub,
                    currentDomainBaseDirectoryGetter);

            var files = fileSystemMock.AllFiles as string[];
            var createdFilePath = files.ToList()
                    .Where(f => f != programPath && f != pendingDataFilePath && f != currentDomainBaseDirectory)
                    .First();
            Assert.Contains(currentDomainBaseDirectory, createdFilePath);
        }

        [Fact]
        public void Execute_CreatesNewFileWithAccountNameInPath_WhenThereIsPendingData()
        {
            var programPath = @"C:\WorldOfWarcraft\_retail_\Wow.exe";
            var accountName = "AccountName";
            var serverName = "ServerName";
            var characterName = "CharacterName";
            var pendingDataFilePath = string.Format(@"C:\WorldOfWarcraft\_retail_\WTF\Account\{0}\{1}\{2}\SavedVariables\TheEyeRecorder.lua",
                    accountName, serverName, characterName);
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) },
                { pendingDataFilePath, new MockFileData(string.Empty) },
            });
            var drivesGetterStub = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformCheckerStub = new OSPlatformCheckerStub(OSPlatform.Windows);
            var currentDomainBaseDirectoryGetter = new CurrentDomainBaseDirectoryGetter(currentDomainBaseDirectory);

            PendingDataConverter.Execute(fileSystemMock, drivesGetterStub, osPlatformCheckerStub,
                    currentDomainBaseDirectoryGetter);

            var files = fileSystemMock.AllFiles as string[];
            var createdFilePath = files.ToList()
                    .Where(f => f != programPath && f != pendingDataFilePath)
                    .First();
            Assert.Contains(accountName, createdFilePath);
        }

        [Theory]
        [InlineData(@"TheEyeTether")]
        [InlineData(@"Data")]
        [InlineData(@"Snapshots")]
        [InlineData(@"AccountName")]
        [InlineData(@"ServerName")]
        public void Execute_CreatesNewFileInCorrectDirectories_WhenThereIsPendingData(string requiredValue)
        {
            var programPath = @"C:\WorldOfWarcraft\_retail_\Wow.exe";
            var pendingDataFilePath = @"C:\WorldOfWarcraft\_retail_\WTF\Account\AccountName\ServerName\CharacterName\SavedVariables\TheEyeRecorder.lua";
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) },
                { pendingDataFilePath, new MockFileData(string.Empty) },
                { currentDomainBaseDirectory, new MockFileData(string.Empty) },
            });
            var drivesGetterStub = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformCheckerStub = new OSPlatformCheckerStub(OSPlatform.Windows);
            var currentDomainBaseDirectoryGetter = new CurrentDomainBaseDirectoryGetter(currentDomainBaseDirectory);

            PendingDataConverter.Execute(fileSystemMock, drivesGetterStub, osPlatformCheckerStub,
                    currentDomainBaseDirectoryGetter);
            
            var files = fileSystemMock.AllFiles as string[];
            var createdFilePath = files.ToList()
                    .Where(f => f != programPath && f != pendingDataFilePath)
                    .First();
            Assert.Contains(requiredValue, createdFilePath);
        }

        [Theory]
        [InlineData(@"C:\TheEyeTether")]
        [InlineData(@"C:\TheEyeTether\Data")]
        [InlineData(@"C:\TheEyeTether\Data\Snapshots")]
        [InlineData(@"C:\TheEyeTether\Data\Snapshots\AccountName")]
        [InlineData(@"C:\TheEyeTether\Data\Snapshots\AccountName\ServerName")]
        public void Execute_CreatesNecessaryDirectories_WhenThereIsPendingData(string requiredValue)
        {
            var programPath = @"C:\WorldOfWarcraft\_retail_\Wow.exe";
            var pendingDataFilePath = @"C:\WorldOfWarcraft\_retail_\WTF\Account\AccountName\ServerName\CharacterName\SavedVariables\TheEyeRecorder.lua";
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) },
                { pendingDataFilePath, new MockFileData(string.Empty) },
                { currentDomainBaseDirectory, new MockFileData(string.Empty) },
            });
            var drivesGetterStub = new DrivesGetterStub(new List<string>() { @"C:\" });
            var osPlatformCheckerStub = new OSPlatformCheckerStub(OSPlatform.Windows);
            var currentDomainBaseDirectoryGetter = new CurrentDomainBaseDirectoryGetter(currentDomainBaseDirectory);

            PendingDataConverter.Execute(fileSystemMock, drivesGetterStub, osPlatformCheckerStub,
                    currentDomainBaseDirectoryGetter);
            
            var directories = fileSystemMock.AllDirectories as string[];
            Assert.Contains(requiredValue, directories);
        }
    }
}
