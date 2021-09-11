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
        private const string AccountName = "VARTIB";
        private const string CharacterName = "Alaror";
        private const string ServerName = "Moonguard";


        [Fact]
        public void Convert_CreatesFiles_WhenThereIsPendingData()
        {
            var programPath = @"C:\WorldOfWarcraft\_retail_\Wow.exe";
            var pendingDataFilePath = string.Format(@"C:\WorldOfWarcraft\_retail_\WTF\Account\{0}\{1}\{2}\SavedVariables\TheEyeRecorder.lua",
                    AccountName, ServerName, CharacterName);
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) },
                { pendingDataFilePath, new MockFileData(string.Empty) }
            });
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
            var stubCurrentDomainBaseDirectoryGetter = new StubCurrentDomainBaseDirectoryGetter(currentDomainBaseDirectory);

            PendingDataConverter.Convert(mockFileSystem, stubDrivesGetter, stubOSPlatformChecker,
                    stubCurrentDomainBaseDirectoryGetter);

            var allFiles = mockFileSystem.AllFiles as string[];
            Assert.Equal(3, allFiles.Length);
        }
        
        [Fact]
        public void Convert_DoesNotThrowError_WhenProgramCannotBeFound()
        {
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>() {});
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
            var stubCurrentDomainBaseDirectoryGetter = new StubCurrentDomainBaseDirectoryGetter(currentDomainBaseDirectory);

            PendingDataConverter.Convert(mockFileSystem, stubDrivesGetter, stubOSPlatformChecker,
                    stubCurrentDomainBaseDirectoryGetter);
        }

        [Fact]
        public void Convert_RemovesFileEndingFromProgramName_WhenCalledOnWindows()
        {
            var programPath = @"C:\WorldOfWarcraft\_retail_\Wow.exe";
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) }
            });
            var stubDivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
            var stubCurrentDomainBaseDirectoryGetter = new StubCurrentDomainBaseDirectoryGetter(currentDomainBaseDirectory);

            PendingDataConverter.Convert(mockFileSystem, stubDivesGetter, stubOSPlatformChecker,
                    stubCurrentDomainBaseDirectoryGetter);
        }

        [Fact]
        public void Convert_RemovesFileEndingFromProgramName_WhenCalledOnMacOS()
        {
            var programPath = @"C:\Applications\WorldOfWarcraft\_retail_\Wow.app";
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) }
            });
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.OSX);
            var stubCurrentDomainBaseDirectoryGetter = new StubCurrentDomainBaseDirectoryGetter(currentDomainBaseDirectory);

            PendingDataConverter.Convert(mockFileSystem, stubDrivesGetter, stubOSPlatformChecker,
                    stubCurrentDomainBaseDirectoryGetter);
        }
        
        [Fact]
        public void Convert_CreatesNewFileWithCurrentDomainBaseDirectoryInPath_WhenThereIsPendingData()
        {
            var programPath = @"C:\WorldOfWarcraft\_retail_\Wow.exe";
            var pendingDataFilePath = string.Format(@"C:\WorldOfWarcraft\_retail_\WTF\Account\{0}\{1}\{2}\SavedVariables\TheEyeRecorder.lua",
                    AccountName, ServerName, CharacterName);
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) },
                { pendingDataFilePath, new MockFileData(string.Empty) },
                { currentDomainBaseDirectory, new MockFileData(string.Empty) },
            });
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
            var stubCurrentDomainBaseDirectoryGetter = new StubCurrentDomainBaseDirectoryGetter(currentDomainBaseDirectory);

            PendingDataConverter.Convert(mockFileSystem, stubDrivesGetter, stubOSPlatformChecker,
                    stubCurrentDomainBaseDirectoryGetter);

            var files = mockFileSystem.AllFiles as string[];
            var createdFilePath = files.ToList()
                    .Where(f => f != programPath && f != pendingDataFilePath && f != currentDomainBaseDirectory)
                    .First();
            Assert.Contains(currentDomainBaseDirectory, createdFilePath);
        }

        [Theory]
        [InlineData(@"TheEyeTether")]
        [InlineData(@"Data")]
        [InlineData(@"Snapshots")]
        [InlineData(AccountName)]
        [InlineData(ServerName)]
        [InlineData(CharacterName)]
        public void Convert_CreatesNewFileInCorrectDirectories_WhenThereIsPendingData(string requiredValue)
        {
            var programPath = @"C:\WorldOfWarcraft\_retail_\Wow.exe";
            var pendingDataFilePath = @"C:\WorldOfWarcraft\_retail_\WTF\Account\AccountName\ServerName\CharacterName\SavedVariables\TheEyeRecorder.lua";
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) },
                { pendingDataFilePath, new MockFileData(string.Empty) },
                { currentDomainBaseDirectory, new MockFileData(string.Empty) },
            });
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
            var stubCurrentDomainBaseDirectoryGetter = new StubCurrentDomainBaseDirectoryGetter(currentDomainBaseDirectory);

            PendingDataConverter.Convert(mockFileSystem, stubDrivesGetter, stubOSPlatformChecker,
                    stubCurrentDomainBaseDirectoryGetter);
            
            var files = mockFileSystem.AllFiles as string[];
            var createdFilePath = files.ToList()
                    .Where(f => f != programPath && f != pendingDataFilePath)
                    .First();
            Assert.Contains(requiredValue, createdFilePath);
        }

        [Theory]
        [InlineData(@"C:\TheEyeTether")]
        [InlineData(@"C:\TheEyeTether\Data")]
        [InlineData(@"C:\TheEyeTether\Data\Snapshots")]
        [InlineData(@"C:\TheEyeTether\Data\Snapshots\" + AccountName)]
        [InlineData(@"C:\TheEyeTether\Data\Snapshots\" + AccountName + @"\"+ ServerName)]
        [InlineData(@"C:\TheEyeTether\Data\Snapshots\" + AccountName + @"\"+ ServerName + @"\"
                + CharacterName)]
        public void Convert_CreatesNecessaryDirectories_WhenThereIsPendingData(string requiredValue)
        {
            var programPath = @"C:\WorldOfWarcraft\_retail_\Wow.exe";
            var pendingDataFilePath = @"C:\WorldOfWarcraft\_retail_\WTF\Account\AccountName\ServerName\CharacterName\SavedVariables\TheEyeRecorder.lua";
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) },
                { pendingDataFilePath, new MockFileData(string.Empty) },
                { currentDomainBaseDirectory, new MockFileData(string.Empty) },
            });
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
            var stubCurrentDomainBaseDirectoryGetter = new StubCurrentDomainBaseDirectoryGetter(currentDomainBaseDirectory);

            PendingDataConverter.Convert(mockFileSystem, stubDrivesGetter, stubOSPlatformChecker,
                    stubCurrentDomainBaseDirectoryGetter);
            
            var directories = mockFileSystem.AllDirectories as string[];
            Assert.Contains(requiredValue, directories);
        }
    }
}
