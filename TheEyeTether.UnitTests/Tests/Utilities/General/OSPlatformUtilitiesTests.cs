using System.Runtime.InteropServices;
using TheEyeTether.UnitTests.Stubs;
using TheEyeTether.Utilities.General;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Utilities.General
{
	public class OSVersionUtilitiesTests
	{
		[Fact]
		public void GetProgramEnding_ThrowsNotImplementedException_WhenCalledOnLinux()
		{
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Linux);

			try
			{
				var result = OSPlatformUtilities.GetProgramEnding(stubOSPlatformChecker);
			}
			catch(System.Exception ex)
			{
				Assert.IsType<System.NotImplementedException>(ex);
			}
		}
		
		[Fact]
		public void GetProgramEnding_ReturnsExe_WhenCalledOnWindows()
		{
			var expectedEnding = ".exe";
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.Windows);

			var result = OSPlatformUtilities.GetProgramEnding(stubOSPlatformChecker);

			Assert.Equal(expectedEnding, result);
		}

		[Fact]
		public void GetProgramEnding_ReturnsApp_WhenCalledOnMacOS()
		{
			var expectedEnding = ".app";
			var stubOSPlatformChecker = new StubOSPlatformChecker(OSPlatform.OSX);

			var result = OSPlatformUtilities.GetProgramEnding(stubOSPlatformChecker);

			Assert.Equal(expectedEnding, result);
		}
	}
}
