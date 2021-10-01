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
            var dataPointStrings2 = new HashSet<string>() { "test2" };
            var hypotheses = new List<Hypothesis>() { new Hypothesis(dataPointStrings1) };

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

        [Fact]
        public void AddUniques_AddsHypothesesToList_WhenListDoesNotHaveHypothesesWithMatchingDataPointStrings()
        {
            var dataPointStrings1 = new HashSet<string>() { "test1" };
            var dataPointStrings2 = new HashSet<string>() { "test2" };
            var dataPointStrings3 = new HashSet<string>() { "test3" };
            var hypotheses = new List<Hypothesis>() { new Hypothesis(dataPointStrings1) };
            var newHypotheses = new List<Hypothesis>()
            {
                new Hypothesis(dataPointStrings2),
                new Hypothesis(dataPointStrings3)
            };

            hypotheses.AddUniques(newHypotheses);

            Assert.Equal(3, hypotheses.Count);
        }

        [Fact]
        public void AddUniques_DoesNotAddHypothesesToList_WhenListHasHypothesesWithMatchingDataPointStrings()
        {
            var dataPointStrings1 = new HashSet<string>() { "test1" };
            var dataPointStrings2 = new HashSet<string>() { "test2" };
            var hypotheses = new List<Hypothesis>()
            {
                new Hypothesis(dataPointStrings1),
                new Hypothesis(dataPointStrings2)
            };

            hypotheses.AddUniques(hypotheses);

            Assert.Equal(2, hypotheses.Count);
        }
    }
}
