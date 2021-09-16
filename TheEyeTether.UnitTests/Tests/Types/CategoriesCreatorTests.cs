using System.Collections.Generic;
using System.Linq;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class CategoriesCreatorTests
    {
        [Fact]
        public void Create_ReturnsADictionaryOfListsOfCategorySettingCategoryPairs_WhenPassedValidLuaTable()
        {
            var categoryName = "test1";
            var subTypeName = "test2";
            var dataPoint = new DataPoint(categoryName, subTypeName, new TimestampRange(1f, 2f));
            var dataPoints = new Dictionary<string, List<DataPoint>>()
            {
                { categoryName, new List<DataPoint>() { dataPoint}}
            };
            var categorySettings = new CategorySetting[] { new CategorySetting(categoryName) };

            var result = CategoriesCreator.Create(dataPoints, categorySettings);

            Assert.IsType<Dictionary<CategorySetting, List<Category>>>(result);
        }

        [Fact]
        public void Create_ThrowsInvalidOperationException_WhenPassedNullDataPoints()
        {
            var categorySettings = new CategorySetting[] { new CategorySetting("test") };

            try
            {
                var result = CategoriesCreator.Create(null, categorySettings);
                Assert.True(false);
            }
            catch(System.Exception ex)
            {
                Assert.IsType<System.InvalidOperationException>(ex);
            }
        }

        [Fact]
        public void Create_ThrowsInvalidOperationException_WhenPassedNullCategorySettings()
        {
            var dataPoints = new Dictionary<string, List<DataPoint>>();

            try
            {
                var result = CategoriesCreator.Create(dataPoints, null);
                Assert.True(false);
            }
            catch(System.Exception ex)
            {
                Assert.IsType<System.InvalidOperationException>(ex);
            }
        }
        
        [Theory]
        [InlineData("test")]
        [InlineData("test1, test2")]
        [InlineData("test1, test2, test3, test4, test5")]
        public void Create_CreatesADictionaryEntryForEachCategorySetting_WhenCategorySettingsArePassedAndHaveValidData(
                params string[] categoryTypeNames)
        {
            var dataPoints = new Dictionary<string, List<DataPoint>>();
            var categorySettings = new CategorySetting[categoryTypeNames.Length];
            for(int i = 0; i < categoryTypeNames.Length; i++)
            {
                var categoryTypeName = categoryTypeNames[i];
                dataPoints[categoryTypeName] = new List<DataPoint>()
                {
                    new DataPoint(categoryTypeName, "test", new TimestampRange(1f, 2f))
                };
                categorySettings[i] = new CategorySetting(categoryTypeName);
            }

            var result = CategoriesCreator.Create(dataPoints, categorySettings);

            Assert.Equal(categoryTypeNames.Length, result.Keys.Count);
        }

        [Theory]
        [InlineData("test")]
        [InlineData("test1", "test2")]
        [InlineData("test1", "test1")]
        [InlineData("test1", "test2", "test3", "test4", "test5")]
        [InlineData("test1", "test1", "test2", "test2", "test3")]
        public void Create_CreatesAListEntryForEachUniqueSubTypeName_WhenCategoryHasMultipleSubTypes(
                params string[] subTypeNames)
        {
            var categoryName = "test";
            var categoryDataPoints = new List<DataPoint>();
            foreach(string subTypeName in subTypeNames)
            {
                categoryDataPoints.Add(new DataPoint(categoryName, subTypeName, new TimestampRange(1f, 2f)));
            }
            var dataPoints = new Dictionary<string, List<DataPoint>>()
            {
                { categoryName, categoryDataPoints }
            };
            var categorySetting = new CategorySetting(categoryName);
            var categorySettings = new CategorySetting[] { categorySetting };

            var result = CategoriesCreator.Create(dataPoints, categorySettings);

            var uniqueSubTypeNames = subTypeNames.Distinct().ToList();
            Assert.Equal(uniqueSubTypeNames.Count, result[categorySetting].Count);
        }
    }
}
