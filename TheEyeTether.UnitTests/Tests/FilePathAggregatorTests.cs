using System.Collections.Generic;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests
{
    public class FilePathAggregatorTests
    {
        [Fact]
        public void AggregateFilePaths_ReturnsListOfStrings_WhenCalled()
        {
            var results = FilePathAggregator.AggregateFilePaths();

            Assert.IsType<List<string>>(results);
        }
    }
}