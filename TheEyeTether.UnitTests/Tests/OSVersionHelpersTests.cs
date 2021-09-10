using System.Runtime.InteropServices;
using TheEyeTether.Helpers;
using TheEyeTether.UnitTests.Stubs;
using Xunit;

namespace TheEyeTether.UnitTests
{
    public class OSVersionHelpersTests
    {
        [Fact]
        public void GetProgramEnding_ThrowsNotImplementedException_WhenCalledOnLinux()
        {
            var osPlatformChecker = new OSPlatformCheckerStub(OSPlatform.Linux);

            try
            {
                var result = OSVersionHelpers.GetProgramEnding(osPlatformChecker);
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
            var osPlatformChecker = new OSPlatformCheckerStub(OSPlatform.Windows);

            var result = OSVersionHelpers.GetProgramEnding(osPlatformChecker);

            Assert.Equal(expectedEnding, result);
        }
    }
}
