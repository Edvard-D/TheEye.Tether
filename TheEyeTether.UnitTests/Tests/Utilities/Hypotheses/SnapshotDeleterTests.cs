using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using TheEye.Tether.UnitTests.Stubs;
using TheEye.Tether.Utilities.Hypotheses;
using Xunit;

namespace TheEye.Tether.UnitTests.Tests.Utilities.Hypotheses
{
	public class SnapshotDeleterTests
	{
		private const string DateTimeFormat = "yyyy_MM_dd__HH_mm_ss";
		private const string NowDateTimeString = "2021_09_23__12_00_00";


		[Theory]
		[InlineData(0)]
		[InlineData(-1)]
		public void DeleteOutdatedFiles_ThrowsInvalidOperationException_WhenPassedNegativeOrZeroLookbackDays(
				int keepLookbackDays)
		{
			var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
					.ToUniversalTime();
			var directoryPath = @"C:\";
			var mockFileSystem = new MockFileSystem();
			var stubClock = new StubClock(nowDateTime);

			try
			{
				SnapshotDeleter.DeleteOutdatedFiles(directoryPath, keepLookbackDays, mockFileSystem,
						stubClock);
				Assert.True(false);
			}
			catch(System.Exception ex)
			{
				Assert.IsType<System.InvalidOperationException>(ex);
			}
		}

		[Fact]
		public void DeleteOutdatedFiles_DoesNotDeleteFilesWithinKeepLookbackDays_WhenCalled()
		{
			var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
					.ToUniversalTime();
			var keepLookbackDays = 1;
			var creationDateTime = nowDateTime.AddDays(-keepLookbackDays);
			var directoryPath = @"C:\";
			var fileName = creationDateTime.ToString(DateTimeFormat) + ".json";
			var mockFileData = new MockFileData(string.Empty);
			mockFileData.CreationTime = creationDateTime;
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ directoryPath + fileName, mockFileData }
			});
			var stubClock = new StubClock(nowDateTime);

			SnapshotDeleter.DeleteOutdatedFiles(directoryPath, keepLookbackDays, mockFileSystem, stubClock);

			Assert.Contains(directoryPath + fileName, mockFileSystem.AllFiles);
		}

		[Fact]
		public void DeleteOutdatedFiles_DeletesFilesOutsideOfKeepLookbackDays_WhenCalled()
		{
			var nowDateTime = System.DateTime.ParseExact(NowDateTimeString, DateTimeFormat, null)
					.ToUniversalTime();
			var keepLookbackDays = 1;
			var creationDateTime = nowDateTime.AddDays(-keepLookbackDays - 1);
			var directoryPath = @"C:\";
			var fileName = creationDateTime.ToString(DateTimeFormat) + ".json";
			var mockFileData = new MockFileData(string.Empty);
			mockFileData.CreationTime = creationDateTime;
			var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
			{
				{ directoryPath + fileName, mockFileData }
			});
			var stubClock = new StubClock(nowDateTime);

			SnapshotDeleter.DeleteOutdatedFiles(directoryPath, keepLookbackDays, mockFileSystem, stubClock);

			Assert.DoesNotContain(directoryPath + fileName, mockFileSystem.AllFiles);
		}
	}
}
