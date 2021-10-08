using TheEyeTether.Utilities.General;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Utilities.General
{
    public class ResourcesTests
    {
        [Fact]
        public void ReadTextResource_ReturnsString_WhenCalled()
        {
            var result = Resources.ReadTextResource();

            Assert.IsType<string>(result);
        }
    }
}
