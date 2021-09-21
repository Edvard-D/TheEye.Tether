using System.Collections.Generic;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class HypothesesCreatorTests
    {
        [Fact]
        public void Create_ReturnsListOfHypotheses_WhenCalled()
        {
            var result = HypothesesCreator.Create();

            Assert.IsType<List<Hypothesis>>(result);
        }
    }
}
