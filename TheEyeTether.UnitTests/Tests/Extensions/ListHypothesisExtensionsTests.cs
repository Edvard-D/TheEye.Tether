using System.Collections.Generic;
using TheEyeTether.Extensions;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Extensions
{
    public class ListHypothesisExtensionsTests
    {
        private const string CategoryId = "testCategoryId";
        private const string CategoryType = "testCategoryType";
        private const string DataPointString = "testDataPointString";
        private const string SnapshotId = "testSnapshotId";
        private const string SnapshotType = "testSnapshotType";


        [Fact]
        public void AddUnique_AddsHypothesisToList_WhenAllValuesMatchExceptCategoryType()
        {
            var dataPointStrings = new HashSet<string>() { DataPointString };
            var hypotheses = new List<Hypothesis>() { new Hypothesis(CategoryType + "1", CategoryId,
                    SnapshotType, SnapshotId, dataPointStrings) };
            var hypothesis = new Hypothesis(CategoryType + "2", CategoryId, SnapshotType, SnapshotId,
                    dataPointStrings);

            hypotheses.AddUnique(hypothesis);

            Assert.Equal(2, hypotheses.Count);
        }

        [Fact]
        public void AddUnique_AddsHypothesisToList_WhenAllValuesMatchExceptCategoryId()
        {
            var dataPointStrings = new HashSet<string>() { DataPointString };
            var hypotheses = new List<Hypothesis>() { new Hypothesis(CategoryType, CategoryId + "1",
                    SnapshotType, SnapshotId, dataPointStrings) };
            var hypothesis = new Hypothesis(CategoryType, CategoryId + "2", SnapshotType, SnapshotId,
                    dataPointStrings);

            hypotheses.AddUnique(hypothesis);

            Assert.Equal(2, hypotheses.Count);
        }

        [Fact]
        public void AddUnique_AddsHypothesisToList_WhenAllValueMatchExceptDataPointStrings()
        {
            var dataPointStrings1 = new HashSet<string>() { DataPointString + "1" };
            var dataPointStrings2 = new HashSet<string>() { DataPointString + "2" };
            var hypotheses = new List<Hypothesis>() { new Hypothesis(CategoryType, CategoryId,
                    SnapshotType, SnapshotId, dataPointStrings1) };

            hypotheses.AddUnique(new Hypothesis(dataPointStrings2));

            Assert.Equal(2, hypotheses.Count);
        }

        [Fact]
        public void AddUnique_DoesNotAddHypothesisToList_WhenAllValuesMatch()
        {
            var dataPointStrings = new HashSet<string>() { DataPointString };
            var hypotheses = new List<Hypothesis>() { new Hypothesis(CategoryType, CategoryId,
                    SnapshotType, SnapshotId, dataPointStrings) };

            hypotheses.AddUnique(hypotheses[0]);

            Assert.Single(hypotheses);
        }

        [Fact]
        public void AddUniques_AddsHypothesesToList_WhenElementWithAllValuesMatchingExceptCategoryTypeExists()
        {
            var dataPointStrings = new HashSet<string>() { DataPointString };
            var hypotheses = new List<Hypothesis>() { new Hypothesis(CategoryType + "1", CategoryId,
                    SnapshotType, SnapshotId, dataPointStrings) };
            var newHypotheses = new List<Hypothesis>()
            {
                new Hypothesis(CategoryType + "2", CategoryId, SnapshotType, SnapshotId, dataPointStrings),
                new Hypothesis(CategoryType + "3", CategoryId, SnapshotType, SnapshotId, dataPointStrings),
            };

            hypotheses.AddUniques(newHypotheses);

            Assert.Equal(3, hypotheses.Count);
        }

        [Fact]
        public void AddUniques_AddsHypothesesToList_WhenElementWithAllValuesMatchingExceptCategoryIdExists()
        {
            var dataPointStrings = new HashSet<string>() { DataPointString };
            var hypotheses = new List<Hypothesis>() { new Hypothesis(CategoryType, CategoryId + "1",
                    SnapshotType, SnapshotId, dataPointStrings) };
            var newHypotheses = new List<Hypothesis>()
            {
                new Hypothesis(CategoryType, CategoryId + "2", SnapshotType, SnapshotId, dataPointStrings),
                new Hypothesis(CategoryType, CategoryId + "3", SnapshotType, SnapshotId, dataPointStrings),
            };

            hypotheses.AddUniques(newHypotheses);

            Assert.Equal(3, hypotheses.Count);
        }

        [Fact]
        public void AddUniques_AddsHypothesesToList_WhenElementWithAllValuesMatchingExceptDataPointStringExists()
        {
            var dataPointStrings1 = new HashSet<string>() { DataPointString + "1" };
            var dataPointStrings2 = new HashSet<string>() { DataPointString + "2" };
            var dataPointStrings3 = new HashSet<string>() { DataPointString + "3" };
            var hypotheses = new List<Hypothesis>() { new Hypothesis(CategoryType, CategoryId,
                    SnapshotType, SnapshotId, dataPointStrings1) };
            var newHypotheses = new List<Hypothesis>()
            {
                new Hypothesis(CategoryType, CategoryId, SnapshotType, SnapshotId, dataPointStrings2),
                new Hypothesis(CategoryType, CategoryId, SnapshotType, SnapshotId, dataPointStrings3),
            };

            hypotheses.AddUniques(newHypotheses);

            Assert.Equal(3, hypotheses.Count);
        }

        [Fact]
        public void AddUniques_DoesNotAddHypothesesToList_WhenElementWithAllValuesMatchingExists()
        {
            var dataPointStrings1 = new HashSet<string>() { DataPointString + "1" };
            var dataPointStrings2 = new HashSet<string>() { DataPointString + "2" };
            var hypotheses = new List<Hypothesis>()
            {
                new Hypothesis(CategoryType, CategoryId, SnapshotType, SnapshotId, dataPointStrings1),
                new Hypothesis(CategoryType, CategoryId, SnapshotType, SnapshotId, dataPointStrings2),
            };

            hypotheses.AddUniques(hypotheses);

            Assert.Equal(2, hypotheses.Count);
        }
    }
}
