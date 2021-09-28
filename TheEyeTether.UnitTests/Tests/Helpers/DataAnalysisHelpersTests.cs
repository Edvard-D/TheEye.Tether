using System.Collections.Generic;
using TheEyeTether.Helpers;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Helpers
{
    public class DataAnalysisHelpersTests
    {
        [Fact]
        public void CalculateJaccardSimilarity_Returns0Percent_WhenAllValuesAreDifferent()
        {
            var listA = new List<int>() { 0, 0, 0, 0, 0 };
            var listB = new List<int>() { 1, 1, 1, 1, 1 };

            var result = DataAnalysisHelpers.CalculateJaccardSimilarity(listA, listB);

            Assert.Equal(0, result);
        }

        [Fact]
        public void CalculateJaccardSimilarity_Returns50Percent_WhenHalfOfValuesAreEqual()
        {
            var listA = new List<int>() { 0, 0, 1, 1 };
            var listB = new List<int>() { 1, 1, 1, 1 };

            var result = DataAnalysisHelpers.CalculateJaccardSimilarity(listA, listB);

            Assert.Equal(0.5f, result);
        }

        [Fact]
        public void CalculateJaccardSimilarity_Returns100Percent_WhenAllValuesAreEqual()
        {
            var listA = new List<int>() { 1, 1, 1, 1, 1 };
            var listB = new List<int>() { 1, 1, 1, 1, 1 };

            var result = DataAnalysisHelpers.CalculateJaccardSimilarity(listA, listB);

            Assert.Equal(1, result);
        }
    }
}
