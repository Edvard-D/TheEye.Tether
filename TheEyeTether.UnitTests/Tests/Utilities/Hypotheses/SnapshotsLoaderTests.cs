using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using TheEye.Tether.Utilities.Hypotheses;
using Xunit;

namespace TheEye.Tether.UnitTests.Tests.Utilities.Hypotheses
{
	public class SnapshotsLoaderTests
	{
		private const string DateTimeFormat = "yyyy_MM_dd__HH_mm_ss";
		private const string NowDateTimeString = "2021_09_23__12_00_00";


		[Fact]
		public void Load_ReturnsListOfListsOfStrings_WhenCalled()
		{
			var directoryPath = @"C:\TestDirectory\";
			var fileName = "2021_09_23__12_00_00.json";
			var snapshotData = new List<List<string>>()
			{
				new List<string>() { "test1" }
			};
			var jsonText = JsonSerializer.Serialize(snapshotData);
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ directoryPath + fileName, new MockFileData(jsonText) }
			});

			var result = SnapshotsLoader.Load(directoryPath, mockFileSystem);

			Assert.IsType<List<List<string>>>(result);
		}

		[Fact]
		public void Load_ReturnsSnapshotsInFilesWithinDirectory_WhenCalled()
		{
			var directoryPath = @"C:\TestDirectory\";
			var fileName1 = "2021_09_23__12_00_00.json";
			var fileName2 = "2021_09_24__12_00_00.json";
			var snapshotData1 = new List<List<string>>()
			{
				new List<string>() { "test1" }
			};
			var snapshotData2 = new List<List<string>>()
			{
				new List<string>() { "test1", "test2", "test3" }
			};
			var jsonText1 = JsonSerializer.Serialize(snapshotData1);
			var jsonText2 = JsonSerializer.Serialize(snapshotData2);
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ directoryPath + fileName1, new MockFileData(jsonText1) },
				{ directoryPath + fileName2, new MockFileData(jsonText2) }
			});

			var result = SnapshotsLoader.Load(directoryPath, mockFileSystem);

			var expectedSnapshotCount = snapshotData1.Count + snapshotData2.Count;
			Assert.Equal(expectedSnapshotCount, result.Count);
		}

		[Fact]
		public void Load_DoesNotThrowException_WhenDirectoryContainsMisformattedFile()
		{
			var directoryPath = @"C:\TestDirectory\";
			var fileName = "2021_09_23__12_00_00.json";
			var snapshotData = new List<string>() { "test" };
			var jsonText = JsonSerializer.Serialize(snapshotData);
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ directoryPath + fileName, new MockFileData(jsonText) }
			});

			var result = SnapshotsLoader.Load(directoryPath, mockFileSystem);
			
			Assert.True(true);
		}
	}
}
