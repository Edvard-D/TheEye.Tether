using System.Collections.Generic;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests
{
    public class ProgramPathLocaterTests
    {
        [Fact]
        public void LocateProgramPath_ReturnsString_WhenCalled()
        {
            var result = ProgramPathLocater.LocateProgramPath();

            Assert.IsType<string>(result);
        }
    }
}
