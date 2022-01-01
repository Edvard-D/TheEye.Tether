using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using TheEye.Tether.Data;
using TheEye.Tether.UnitTests.Mocks;
using TheEye.Tether.UnitTests.Stubs;
using TheEye.Tether.Utilities.Hypotheses;
using Xunit;


namespace TheEye.Tether.UnitTests.Tests.Utilities.Hypotheses
{
	public class PendingDataConverterTests
	{
		private const string AccountName = "VARTIB";
		private const string CharacterName = "Alaror";
		private const string CategorySettingName = "PLAYER_SPECIALIZATION";
		private const string CurrentDomainBaseDirectory = @"C:\TheEyeTether\";
		private const string DataPointSubTypeName = "true";
		private const string DataPointTypeName = "PLAYER_HAS_TARGET";
		private const string DateTimeFormat = "yyyy_MM_dd__HH_mm_ss";
		private const string FileSaveDateTime = "2021_09_20__12_00_00";
		private const string LuaFileText =  "TheEyeRecorder_Data = { "
				+ "[\""+CategorySettingName+"\"] = { [\""+SpecializationId+"\"] = { 10000.001 } }, "
				+ "[\""+SnapshotSettingName+"\"] = { [\""+SnapshotSubTypeName+"\"] = { 10000.001 } }, "
				+ "[\""+DataPointTypeName+"\"] = { [\""+DataPointSubTypeName+"\"] = { 10000.001 } } "
				+ "}";
		private const string MainDirectory = @"C:\World of Warcraft\_retail_\";
		private const string ProgramPath = MainDirectory + "Wow.exe";
		private const string ServerName = "Moonguard";
		private const string SnapshotSettingName = "PLAYER_SPELLCAST_START";
		private const string SnapshotSubTypeName = "1000";
		private const string SpecializationId = "100";


		[Fact]
		public void Convert_CreatesFiles_WhenThereIsPendingData()
		{
			var pendingDataFilePath = string.Format(MainDirectory + @"WTF\Account\{0}\{1}\{2}\SavedVariables\TheEyeRecorder.lua",
					AccountName, ServerName, CharacterName);
			var mockLua = new StubLua(CurrentDomainBaseDirectory, new Dictionary<string, string>()
			{
				{ pendingDataFilePath, LuaFileText}
			});
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ ProgramPath, new MockFileData(string.Empty) },
				{ pendingDataFilePath, LuaFileText },
				{ CurrentDomainBaseDirectory, new MockFileData(string.Empty) }
			});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
			var stubCurrentDomainBaseDirectoryProvider = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);
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
				{ DataPointTypeName, new DataPointSetting("false", 0, new int[0]) }
			};
			var stubClock = new StubClock(System.DateTime.ParseExact(FileSaveDateTime, DateTimeFormat,
					null));

			PendingDataConverter.Convert(categorySettings, dataPointSettings, mockFileSystem, mockLua,
					stubDrivesProvider, stubOSPlatformChecker, stubCurrentDomainBaseDirectoryProvider,
					stubClock);

			var allFiles = mockFileSystem.AllFiles as string[];
			Assert.Equal(3, allFiles.Length);
		}
		
		[Fact]
		public void Convert_DoesNotThrowError_WhenProgramCannotBeFound()
		{
			var mockLua = new StubLua(CurrentDomainBaseDirectory, new Dictionary<string, string>());
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>() {});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
			var stubCurrentDomainBaseDirectoryProvider = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);
			var categorySettings = new Dictionary<string, CategorySetting>();
			var dataPointSettings = new Dictionary<string, DataPointSetting>();
			var stubClock = new StubClock(System.DateTime.ParseExact(FileSaveDateTime, DateTimeFormat,
					null));

			PendingDataConverter.Convert(categorySettings, dataPointSettings, mockFileSystem, mockLua,
					stubDrivesProvider, stubOSPlatformChecker, stubCurrentDomainBaseDirectoryProvider,
					stubClock);
		}

		[Fact]
		public void Convert_RemovesFileEndingFromProgramName_WhenCalledOnWindows()
		{
			var pendingDataFilePath = string.Format(MainDirectory + @"WTF\Account\{0}\{1}\{2}\SavedVariables\TheEyeRecorder.lua",
					AccountName, ServerName, CharacterName);
			var mockLua = new StubLua(CurrentDomainBaseDirectory, new Dictionary<string, string>()
			{
				{ pendingDataFilePath, LuaFileText}
			});
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ ProgramPath, new MockFileData(string.Empty) },
				{ pendingDataFilePath, LuaFileText },
				{ CurrentDomainBaseDirectory, new MockFileData(string.Empty) }
			});
			var stubDivesGetter = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
			var stubCurrentDomainBaseDirectoryProvider = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);
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
				{ DataPointTypeName, new DataPointSetting("false", 0, new int[0]) }
			};
			var stubClock = new StubClock(System.DateTime.ParseExact(FileSaveDateTime, DateTimeFormat,
					null));

			PendingDataConverter.Convert(categorySettings, dataPointSettings, mockFileSystem, mockLua,
					stubDivesGetter, stubOSPlatformChecker, stubCurrentDomainBaseDirectoryProvider,
					stubClock);

			Assert.True(true);
		}

		[Fact]
		public void Convert_RemovesFileEndingFromProgramName_WhenCalledOnMacOS()
		{
			var pendingDataFilePath = string.Format(MainDirectory + @"WTF\Account\{0}\{1}\{2}\SavedVariables\TheEyeRecorder.lua",
					AccountName, ServerName, CharacterName);
			var mockLua = new StubLua(CurrentDomainBaseDirectory, new Dictionary<string, string>()
			{
				{ pendingDataFilePath, LuaFileText}
			});
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ ProgramPath, new MockFileData(string.Empty) },
				{ pendingDataFilePath, LuaFileText },
				{ CurrentDomainBaseDirectory, new MockFileData(string.Empty) }
			});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.OSX);
			var stubCurrentDomainBaseDirectoryProvider = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);
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
				{ DataPointTypeName, new DataPointSetting("false", 0, new int[0]) }
			};
			var stubClock = new StubClock(System.DateTime.ParseExact(FileSaveDateTime, DateTimeFormat,
					null));

			PendingDataConverter.Convert(categorySettings, dataPointSettings, mockFileSystem, mockLua,
					stubDrivesProvider, stubOSPlatformChecker, stubCurrentDomainBaseDirectoryProvider,
					stubClock);

			Assert.True(true);
		}
		
		[Fact]
		public void Convert_CreatesNewFileWithCurrentDomainBaseDirectoryInPath_WhenThereIsPendingData()
		{
			var pendingDataFilePath = string.Format(MainDirectory + @"WTF\Account\{0}\{1}\{2}\SavedVariables\TheEyeRecorder.lua",
					AccountName, ServerName, CharacterName);
			var mockLua = new StubLua(CurrentDomainBaseDirectory, new Dictionary<string, string>()
			{
				{ pendingDataFilePath, LuaFileText}
			});
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ ProgramPath, new MockFileData(string.Empty) },
				{ pendingDataFilePath, LuaFileText },
				{ CurrentDomainBaseDirectory, new MockFileData(string.Empty) },
			});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
			var stubCurrentDomainBaseDirectoryProvider = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);
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
				{ DataPointTypeName, new DataPointSetting("false", 0, new int[0]) }
			};
			var stubClock = new StubClock(System.DateTime.ParseExact(FileSaveDateTime, DateTimeFormat,
					null));

			PendingDataConverter.Convert(categorySettings, dataPointSettings, mockFileSystem, mockLua,
					stubDrivesProvider, stubOSPlatformChecker, stubCurrentDomainBaseDirectoryProvider,
					stubClock);

			var files = mockFileSystem.AllFiles as string[];
			var createdFilePath = files.ToList()
					.Where(f => f != ProgramPath && f != pendingDataFilePath && f != CurrentDomainBaseDirectory)
					.First();
			Assert.Contains(CurrentDomainBaseDirectory, createdFilePath);
		}

		[Theory]
		[InlineData("TheEyeTether")]
		[InlineData("Data")]
		[InlineData("Snapshots")]
		[InlineData(CategorySettingName)]
		[InlineData(SpecializationId)]
		[InlineData(SnapshotSettingName)]
		[InlineData(SnapshotSubTypeName)]
		[InlineData(FileSaveDateTime)]
		public void Convert_CreatesNewFileInCorrectDirectories_WhenThereIsPendingData(string requiredValue)
		{
			var pendingDataFilePath = string.Format(MainDirectory + @"WTF\Account\{0}\{1}\{2}\SavedVariables\TheEyeRecorder.lua",
					AccountName, ServerName, CharacterName);
			var mockLua = new StubLua(CurrentDomainBaseDirectory, new Dictionary<string, string>()
			{
				{ pendingDataFilePath, LuaFileText }
			});
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ ProgramPath, new MockFileData(string.Empty) },
				{ pendingDataFilePath, LuaFileText },
				{ CurrentDomainBaseDirectory, new MockFileData(string.Empty) },
			});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
			var stubCurrentDomainBaseDirectoryProvider = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);
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
				{ DataPointTypeName, new DataPointSetting("false", 0, new int[0]) }
			};
			var stubClock = new StubClock(System.DateTime.ParseExact(FileSaveDateTime, DateTimeFormat,
					null));

			PendingDataConverter.Convert(categorySettings, dataPointSettings, mockFileSystem, mockLua,
					stubDrivesProvider, stubOSPlatformChecker, stubCurrentDomainBaseDirectoryProvider,
					stubClock);
			
			var files = mockFileSystem.AllFiles as string[];
			var createdFilePath = files.ToList()
					.Where(f => f != ProgramPath && f != pendingDataFilePath)
					.First();
			Assert.Contains(requiredValue, createdFilePath);
		}

		[Theory]
		[InlineData(@"C:\TheEyeTether")]
		[InlineData(@"C:\TheEyeTether\Data")]
		[InlineData(@"C:\TheEyeTether\Data\Snapshots")]
		[InlineData(@"C:\TheEyeTether\Data\Snapshots\"+CategorySettingName)]
		[InlineData(@"C:\TheEyeTether\Data\Snapshots\"+CategorySettingName+@"\"+SpecializationId)]
		[InlineData(@"C:\TheEyeTether\Data\Snapshots\"+CategorySettingName+@"\"+SpecializationId+@"\"+
				SnapshotSettingName)]
		[InlineData(@"C:\TheEyeTether\Data\Snapshots\"+CategorySettingName+@"\"+SpecializationId+@"\"+
				SnapshotSettingName+@"\"+SnapshotSubTypeName)]
		public void Convert_CreatesNecessaryDirectories_WhenThereIsPendingData(string requiredValue)
		{
			var pendingDataFilePath = string.Format(MainDirectory + @"WTF\Account\{0}\{1}\{2}\SavedVariables\TheEyeRecorder.lua",
					AccountName, ServerName, CharacterName);
			var mockLua = new StubLua(CurrentDomainBaseDirectory, new Dictionary<string, string>()
			{
				{ pendingDataFilePath, LuaFileText}
			});
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ ProgramPath, new MockFileData(string.Empty) },
				{ pendingDataFilePath, LuaFileText },
				{ CurrentDomainBaseDirectory, new MockFileData(string.Empty) },
			});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
			var stubCurrentDomainBaseDirectoryProvider = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);
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
				{ DataPointTypeName, new DataPointSetting("false", 0, new int[0]) }
			};
			var stubClock = new StubClock(System.DateTime.ParseExact(FileSaveDateTime, DateTimeFormat,
					null));

			PendingDataConverter.Convert(categorySettings, dataPointSettings, mockFileSystem, mockLua,
					stubDrivesProvider, stubOSPlatformChecker, stubCurrentDomainBaseDirectoryProvider,
					stubClock);
			
			var directories = mockFileSystem.AllDirectories as string[];
			Assert.Contains(requiredValue, directories);
		}

		[Fact]
		public void Convert_FormatsDataAsJson_WhenThereIsPendingData()
		{
			var pendingDataFilePath = string.Format(MainDirectory + @"WTF\Account\{0}\{1}\{2}\SavedVariables\TheEyeRecorder.lua",
					AccountName, ServerName, CharacterName);
			var mockLua = new StubLua(CurrentDomainBaseDirectory, new Dictionary<string, string>()
			{
				{ pendingDataFilePath, LuaFileText}
			});
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ ProgramPath, new MockFileData(string.Empty) },
				{ pendingDataFilePath, LuaFileText },
				{ CurrentDomainBaseDirectory, new MockFileData(string.Empty) },
			});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
			var stubCurrentDomainBaseDirectoryProvider = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);
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
				{ DataPointTypeName, new DataPointSetting("false", 0, new int[0]) }
			};
			var stubClock = new StubClock(System.DateTime.ParseExact(FileSaveDateTime, DateTimeFormat,
					null));

			PendingDataConverter.Convert(categorySettings, dataPointSettings, mockFileSystem, mockLua,
					stubDrivesProvider, stubOSPlatformChecker, stubCurrentDomainBaseDirectoryProvider,
					stubClock);

			var outputFilePath = @"C:\TheEyeTether\Data\Snapshots\"+CategorySettingName+@"\"+
					SpecializationId+@"\"+SnapshotSettingName+@"\"+SnapshotSubTypeName+@"\"+
					FileSaveDateTime+".json";
			var outputFile = mockFileSystem.File.ReadAllText(outputFilePath);
			var outputJson = JsonSerializer.Deserialize<List<List<string>>>(outputFile);
			Assert.Equal(DataPointTypeName + "__" + DataPointSubTypeName, outputJson[0][0]);
		}

		[Fact]
		public void Convert_DeletesConvertedPendingData_WhenCalled()
		{
			var pendingDataFilePath = string.Format(MainDirectory + @"WTF\Account\{0}\{1}\{2}\SavedVariables\TheEyeRecorder.lua",
					AccountName, ServerName, CharacterName);
			var mockLua = new StubLua(CurrentDomainBaseDirectory, new Dictionary<string, string>()
			{
				{ pendingDataFilePath, LuaFileText}
			});
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ ProgramPath, new MockFileData(string.Empty) },
				{ pendingDataFilePath, LuaFileText },
				{ CurrentDomainBaseDirectory, new MockFileData(string.Empty) },
			});
			var stubDrivesProvider = new StubDrivesProvider(new List<string>() { @"C:\" });
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);
			var stubCurrentDomainBaseDirectoryProvider = new StubCurrentDomainBaseDirectoryProvider(
					CurrentDomainBaseDirectory);
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
				{ DataPointTypeName, new DataPointSetting("false", 0, new int[0]) }
			};
			var stubClock = new StubClock(System.DateTime.ParseExact(FileSaveDateTime, DateTimeFormat,
					null));

			PendingDataConverter.Convert(categorySettings, dataPointSettings, mockFileSystem, mockLua,
					stubDrivesProvider, stubOSPlatformChecker, stubCurrentDomainBaseDirectoryProvider,
					stubClock);

			Assert.False(mockFileSystem.FileExists(pendingDataFilePath));
		}
	}
}
