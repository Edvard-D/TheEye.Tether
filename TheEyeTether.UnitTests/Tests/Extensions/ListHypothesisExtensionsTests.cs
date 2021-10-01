using System.Collections.Generic;
using TheEyeTether.Extensions;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Extensions
{
    public class ListHypothesisExtensionsTests
    {
        [Fact]
        public void AddUnique_AddsHypothesisToList_WhenListDoesNotHaveAHypothesisWithMatchingDataPointStrings()
        {
            var dataPointStrings1 = new HashSet<string>() { "test1" };
            var hypotheses = new List<Hypothesis>() { new Hypothesis(dataPointStrings1) };

            var dataPointStrings2 = new HashSet<string>() { "test2" };
            hypotheses.AddUnique(new Hypothesis(dataPointStrings2));

            Assert.Equal(2, hypotheses.Count);
        }

        [Fact]
        public void AddUnique_DoesNotAddHypothesisToList_WhenListDoesHaveAHypothesisWithMatchingDataPointStrings()
        {
            var dataPointStrings1 = new HashSet<string>() { "test1" };
            var hypotheses = new List<Hypothesis>() { new Hypothesis(dataPointStrings1) };

            hypotheses.AddUnique(new Hypothesis(dataPointStrings1));

            Assert.Equal(1, hypotheses.Count);
        }
    }
}
