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
            var categorySettings = new Dictionary<string, CategorySetting>
            {
                { categoryName, new CategorySetting(categoryName, null) }
            };

            var result = CategoriesCreator.Create(dataPoints, categorySettings);

            Assert.IsType<Dictionary<CategorySetting, List<Category>>>(result);
        }

        [Fact]
        public void Create_ThrowsInvalidOperationException_WhenPassedNullDataPoints()
        {
            var categoryName = "test";
            var categorySettings = new Dictionary<string, CategorySetting>
            {
                { categoryName, new CategorySetting(categoryName, null) }
            };

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
            var categorySettings = new Dictionary<string, CategorySetting>();
            for(int i = 0; i < categoryTypeNames.Length; i++)
            {
                var categoryTypeName = categoryTypeNames[i];
                dataPoints[categoryTypeName] = new List<DataPoint>()
                {
                    new DataPoint(categoryTypeName, "test", new TimestampRange(1f, 2f))
                };
                categorySettings[categoryTypeNames[i]] = new CategorySetting(categoryTypeName, null);
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
            var categorySetting = new CategorySetting(categoryName, null);
            var categorySettings = new Dictionary<string, CategorySetting>
            {
                { categoryName, categorySetting }
            };

            var result = CategoriesCreator.Create(dataPoints, categorySettings);

            var uniqueSubTypeNames = subTypeNames.Distinct().ToList();
            Assert.Equal(uniqueSubTypeNames.Count, result[categorySetting].Count);
        }

        [Fact]
        public void Create_AssignsSubTypeNameAsCategoryIdentifier_WhenCalled()
        {
            var categoryName = "test1";
            var subTypeName = "test2";
            var dataPoints = new Dictionary<string, List<DataPoint>>()
            {
                {
                    categoryName, new List<DataPoint>()
                    {
                        new DataPoint(categoryName, subTypeName, new TimestampRange(1f, 2f))
                    }
                }
            };
            var categorySetting = new CategorySetting(categoryName, null);
            var categorySettings = new Dictionary<string, CategorySetting>
            {
                { categoryName, categorySetting }
            };

            var result = CategoriesCreator.Create(dataPoints, categorySettings);

            Assert.Equal(subTypeName, result[categorySetting][0].Identifier);
        }

        [Fact]
        public void Create_AssignsCategorySettingAsCategorySetting_WhenCalled()
        {
            var categoryName = "test1";
            var subTypeName = "test2";
            var dataPoints = new Dictionary<string, List<DataPoint>>()
            {
                {
                    categoryName, new List<DataPoint>()
                    {
                        new DataPoint(categoryName, subTypeName, new TimestampRange(1f, 2f))
                    }
                }
            };
            var categorySetting = new CategorySetting(categoryName, null);
            var categorySettings = new Dictionary<string, CategorySetting>
            {
                { categoryName, categorySetting }
            };

            var result = CategoriesCreator.Create(dataPoints, categorySettings);

            Assert.Equal(categorySetting, result[categorySetting][0].Setting);
        }

        [Fact]
        public void Create_DoesNotCreateDictionaryEntry_WhenCategoryHasNoData()
        {
            var categoryName = "test";
            var dataPoints = new Dictionary<string, List<DataPoint>>();
            var categorySetting = new CategorySetting(categoryName, null);
            var categorySettings = new Dictionary<string, CategorySetting>
            {
                { categoryName, categorySetting }
            };

            var result = CategoriesCreator.Create(dataPoints, categorySettings);

            Assert.DoesNotContain(categorySetting, result.Keys);
        }

        [Theory]
        [InlineData(1f, 2f)]
        [InlineData(1f, 2f, 3f)]
        [InlineData(1f, 2f, 3f, 4f, 5f)]
        public void Create_AddsEntryToActiveTimePeriodsForEachTimestampRange_WhenMultipleDataPointsExistWithTheSameSubTypeName(
                params float[] timestamps)
        {
            var categoryName = "test1";
            var subTypeName = "test2";
            var categoryDataPoints = new List<DataPoint>();
            for(int i = 0; i < timestamps.Length; i++)
            {
                float endTimestamp = float.MaxValue;
                if(i + 1 < timestamps.Length)
                {
                    endTimestamp = timestamps[i + 1];
                }

                categoryDataPoints.Add(new DataPoint(categoryName, subTypeName,
                        new TimestampRange(timestamps[i], endTimestamp)));
            }
            var dataPoints = new Dictionary<string, List<DataPoint>>()
            {
                { categoryName, categoryDataPoints }
            };
            var categorySetting = new CategorySetting(categoryName, null);
            var categorySettings = new Dictionary<string, CategorySetting>
            {
                { categoryName, categorySetting }
            };

            var result = CategoriesCreator.Create(dataPoints, categorySettings);

            Assert.Equal(timestamps.Length, result[categorySetting][0].ActiveTimePeriods.Count);
        }
    }
}
