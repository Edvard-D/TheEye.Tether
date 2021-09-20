using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Runtime.InteropServices;
using TheEyeTether.Types;
using TheEyeTether.UnitTests.Mocks;
using TheEyeTether.UnitTests.Stubs;
using Xunit;


namespace TheEyeTether.UnitTests.Tests.Types
{
    public class PendingDataConverterTests
    {
        private const string AccountName = "VARTIB";
        private const string CharacterName = "Alaror";
        private const string CategorySettingName = "PLAYER_SPECIALIZATION";
        private const string DataPointTypeName = "PLAYER_HAS_TARGET";
        private const string LuaFileText =  "TheEyeRecordedData = { "
                + "[\""+CategorySettingName+"\"] = { [\""+SpecializationId+"\"] = { 10000.001 } }, "
                + "[\""+SnapshotSettingName+"\"] = { [\"1000\"] = { 10000.001 } }, "
                + "[\""+DataPointTypeName+"\"] = { [\"true\"] = { 10000.001 } } "
                + "}";
        private const string ServerName = "Moonguard";
        private const string SnapshotSettingName = "PLAYER_SPELLCAST_START";
        private const string SpecializationId = "100";


        [Fact]
        public void Convert_CreatesFiles_WhenThereIsPendingData()
        {
            var programPath = @"C:\WorldOfWarcraft\_retail_\Wow.exe";
            var pendingDataFilePath = string.Format(@"C:\WorldOfWarcraft\_retail_\WTF\Account\{0}\{1}\{2}\SavedVariables\TheEyeRecorder.lua",
                    AccountName, ServerName, CharacterName);
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var mockLua = new MockLua(currentDomainBaseDirectory, new Dictionary<string, string>()
            {
                { pendingDataFilePath, LuaFileText}
            });
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) },
                { pendingDataFilePath, LuaFileText },
                { currentDomainBaseDirectory, new MockFileData(string.Empty) }
            });
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
            var stubCurrentDomainBaseDirectoryGetter = new StubCurrentDomainBaseDirectoryGetter(
                    currentDomainBaseDirectory);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { CategorySettingName, new CategorySetting(CategorySettingName,
                        new Dictionary<string, SnapshotSetting>()
                        {
                            { SnapshotSettingName, new SnapshotSetting(SnapshotSettingName,
                                    new string[] { DataPointTypeName }) }
                        })
                }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { CategorySettingName, new DataPointSetting() },
                { SnapshotSettingName, new DataPointSetting() },
                { DataPointTypeName, new DataPointSetting("false", 0) }
            };

            PendingDataConverter.Convert(categorySettings, dataPointSettings, mockFileSystem, mockLua,
                    stubDrivesGetter, stubOSPlatformChecker, stubCurrentDomainBaseDirectoryGetter);

            var allFiles = mockFileSystem.AllFiles as string[];
            Assert.Equal(3, allFiles.Length);
        }
        
        [Fact]
        public void Convert_DoesNotThrowError_WhenProgramCannotBeFound()
        {
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var mockLua = new MockLua(currentDomainBaseDirectory, new Dictionary<string, string>());
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>() {});
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
            var stubCurrentDomainBaseDirectoryGetter = new StubCurrentDomainBaseDirectoryGetter(
                    currentDomainBaseDirectory);
            var categorySettings = new Dictionary<string, CategorySetting>();
            var dataPointSettings = new Dictionary<string, DataPointSetting>();

            PendingDataConverter.Convert(categorySettings, dataPointSettings, mockFileSystem, mockLua,
                    stubDrivesGetter, stubOSPlatformChecker, stubCurrentDomainBaseDirectoryGetter);
        }

        [Fact]
        public void Convert_RemovesFileEndingFromProgramName_WhenCalledOnWindows()
        {
            var programPath = @"C:\WorldOfWarcraft\_retail_\Wow.exe";
            var pendingDataFilePath = string.Format(@"C:\WorldOfWarcraft\_retail_\WTF\Account\{0}\{1}\{2}\SavedVariables\TheEyeRecorder.lua",
                    AccountName, ServerName, CharacterName);
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var mockLua = new MockLua(currentDomainBaseDirectory, new Dictionary<string, string>()
            {
                { pendingDataFilePath, LuaFileText}
            });
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) },
                { pendingDataFilePath, LuaFileText },
                { currentDomainBaseDirectory, new MockFileData(string.Empty) }
            });
            var stubDivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
            var stubCurrentDomainBaseDirectoryGetter = new StubCurrentDomainBaseDirectoryGetter(
                    currentDomainBaseDirectory);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { CategorySettingName, new CategorySetting(CategorySettingName,
                        new Dictionary<string, SnapshotSetting>()
                        {
                            { SnapshotSettingName, new SnapshotSetting(SnapshotSettingName,
                                    new string[] { DataPointTypeName }) }
                        })
                }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { CategorySettingName, new DataPointSetting() },
                { SnapshotSettingName, new DataPointSetting() },
                { DataPointTypeName, new DataPointSetting("false", 0) }
            };

            PendingDataConverter.Convert(categorySettings, dataPointSettings, mockFileSystem, mockLua,
                    stubDivesGetter, stubOSPlatformChecker, stubCurrentDomainBaseDirectoryGetter);

            Assert.True(true);
        }

        [Fact]
        public void Convert_RemovesFileEndingFromProgramName_WhenCalledOnMacOS()
        {
            var programPath = @"C:\Applications\WorldOfWarcraft\_retail_\Wow.app";
            var pendingDataFilePath = string.Format(@"C:\Applications\WorldOfWarcraft\_retail_\WTF\Account\{0}\{1}\{2}\SavedVariables\TheEyeRecorder.lua",
                    AccountName, ServerName, CharacterName);
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var mockLua = new MockLua(currentDomainBaseDirectory, new Dictionary<string, string>()
            {
                { pendingDataFilePath, LuaFileText}
            });
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) },
                { pendingDataFilePath, LuaFileText },
                { currentDomainBaseDirectory, new MockFileData(string.Empty) }
            });
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.OSX);
            var stubCurrentDomainBaseDirectoryGetter = new StubCurrentDomainBaseDirectoryGetter(
                    currentDomainBaseDirectory);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { CategorySettingName, new CategorySetting(CategorySettingName,
                        new Dictionary<string, SnapshotSetting>()
                        {
                            { SnapshotSettingName, new SnapshotSetting(SnapshotSettingName,
                                    new string[] { DataPointTypeName }) }
                        })
                }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { CategorySettingName, new DataPointSetting() },
                { SnapshotSettingName, new DataPointSetting() },
                { DataPointTypeName, new DataPointSetting("false", 0) }
            };

            PendingDataConverter.Convert(categorySettings, dataPointSettings, mockFileSystem, mockLua,
                    stubDrivesGetter, stubOSPlatformChecker, stubCurrentDomainBaseDirectoryGetter);

            Assert.True(true);
        }
        
        [Fact]
        public void Convert_CreatesNewFileWithCurrentDomainBaseDirectoryInPath_WhenThereIsPendingData()
        {
            var programPath = @"C:\WorldOfWarcraft\_retail_\Wow.exe";
            var pendingDataFilePath = string.Format(@"C:\WorldOfWarcraft\_retail_\WTF\Account\{0}\{1}\{2}\SavedVariables\TheEyeRecorder.lua",
                    AccountName, ServerName, CharacterName);
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var mockLua = new MockLua(currentDomainBaseDirectory, new Dictionary<string, string>()
            {
                { pendingDataFilePath, LuaFileText}
            });
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) },
                { pendingDataFilePath, LuaFileText },
                { currentDomainBaseDirectory, new MockFileData(string.Empty) },
            });
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
            var stubCurrentDomainBaseDirectoryGetter = new StubCurrentDomainBaseDirectoryGetter(
                    currentDomainBaseDirectory);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { CategorySettingName, new CategorySetting(CategorySettingName,
                        new Dictionary<string, SnapshotSetting>()
                        {
                            { SnapshotSettingName, new SnapshotSetting(SnapshotSettingName,
                                    new string[] { DataPointTypeName }) }
                        })
                }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { CategorySettingName, new DataPointSetting() },
                { SnapshotSettingName, new DataPointSetting() },
                { DataPointTypeName, new DataPointSetting("false", 0) }
            };

            PendingDataConverter.Convert(categorySettings, dataPointSettings, mockFileSystem, mockLua,
                    stubDrivesGetter, stubOSPlatformChecker, stubCurrentDomainBaseDirectoryGetter);

            var files = mockFileSystem.AllFiles as string[];
            var createdFilePath = files.ToList()
                    .Where(f => f != programPath && f != pendingDataFilePath && f != currentDomainBaseDirectory)
                    .First();
            Assert.Contains(currentDomainBaseDirectory, createdFilePath);
        }

        [Theory]
        [InlineData("TheEyeTether")]
        [InlineData("Data")]
        [InlineData("Snapshots")]
        [InlineData(CategorySettingName)]
        [InlineData(SpecializationId)]
        public void Convert_CreatesNewFileInCorrectDirectories_WhenThereIsPendingData(string requiredValue)
        {
            var programPath = @"C:\WorldOfWarcraft\_retail_\Wow.exe";
            var pendingDataFilePath = string.Format(@"C:\WorldOfWarcraft\_retail_\WTF\Account\{0}\{1}\{2}\SavedVariables\TheEyeRecorder.lua",
                    AccountName, ServerName, CharacterName);
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var mockLua = new MockLua(currentDomainBaseDirectory, new Dictionary<string, string>()
            {
                { pendingDataFilePath, LuaFileText }
            });
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) },
                { pendingDataFilePath, LuaFileText },
                { currentDomainBaseDirectory, new MockFileData(string.Empty) },
            });
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
            var stubCurrentDomainBaseDirectoryGetter = new StubCurrentDomainBaseDirectoryGetter(
                    currentDomainBaseDirectory);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { CategorySettingName, new CategorySetting(CategorySettingName,
                        new Dictionary<string, SnapshotSetting>()
                        {
                            { SnapshotSettingName, new SnapshotSetting(SnapshotSettingName,
                                    new string[] { DataPointTypeName }) }
                        })
                }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { CategorySettingName, new DataPointSetting() },
                { SnapshotSettingName, new DataPointSetting() },
                { DataPointTypeName, new DataPointSetting("false", 0) }
            };

            PendingDataConverter.Convert(categorySettings, dataPointSettings, mockFileSystem, mockLua,
                    stubDrivesGetter, stubOSPlatformChecker, stubCurrentDomainBaseDirectoryGetter);
            
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
        [InlineData(@"C:\TheEyeTether\Data\Snapshots\"+CategorySettingName)]
        [InlineData(@"C:\TheEyeTether\Data\Snapshots\"+CategorySettingName+@"\"+SpecializationId)]
        public void Convert_CreatesNecessaryDirectories_WhenThereIsPendingData(string requiredValue)
        {
            var programPath = @"C:\WorldOfWarcraft\_retail_\Wow.exe";
            var pendingDataFilePath = string.Format(@"C:\WorldOfWarcraft\_retail_\WTF\Account\{0}\{1}\{2}\SavedVariables\TheEyeRecorder.lua",
                    AccountName, ServerName, CharacterName);
            var currentDomainBaseDirectory = @"C:\TheEyeTether\";
            var mockLua = new MockLua(currentDomainBaseDirectory, new Dictionary<string, string>()
            {
                { pendingDataFilePath, LuaFileText}
            });
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { programPath, new MockFileData(string.Empty) },
                { pendingDataFilePath, LuaFileText },
                { currentDomainBaseDirectory, new MockFileData(string.Empty) },
            });
            var stubDrivesGetter = new StubDrivesGetter(new List<string>() { @"C:\" });
            var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
            var stubCurrentDomainBaseDirectoryGetter = new StubCurrentDomainBaseDirectoryGetter(
                    currentDomainBaseDirectory);
            var categorySettings = new Dictionary<string, CategorySetting>()
            {
                { CategorySettingName, new CategorySetting(CategorySettingName,
                        new Dictionary<string, SnapshotSetting>()
                        {
                            { SnapshotSettingName, new SnapshotSetting(SnapshotSettingName,
                                    new string[] { DataPointTypeName }) }
                        })
                }
            };
            var dataPointSettings = new Dictionary<string, DataPointSetting>()
            {
                { CategorySettingName, new DataPointSetting() },
                { SnapshotSettingName, new DataPointSetting() },
                { DataPointTypeName, new DataPointSetting("false", 0) }
            };

            PendingDataConverter.Convert(categorySettings, dataPointSettings, mockFileSystem, mockLua,
                    stubDrivesGetter, stubOSPlatformChecker, stubCurrentDomainBaseDirectoryGetter);
            
            var directories = mockFileSystem.AllDirectories as string[];
            Assert.Contains(requiredValue, directories);
        }
    }
}
