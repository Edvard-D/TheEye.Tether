using System.Collections.Generic;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class CategoriesCreatorTests
    {
        [Fact]
        public void Create_ReturnsADictionaryOfCategorySettingCategoryPairs_WhenPassedValidLuaTable()
        {
            var categoryName = "test1";
            var subTableName = "test2";
            var subTable = new Dictionary<object, object>()
            {
                { subTableName, new Dictionary<object, object>() { { 1, 1f } } }
            };
            var luaTable = new Dictionary<object, object>() { { categoryName, subTable } };
            var categorySettings = new CategorySetting[] { new CategorySetting(categoryName) };

            var result = CategoriesCreator.Create(luaTable, categorySettings);

            Assert.IsType<Dictionary<CategorySetting, Category>>(result);
        }

        [Fact]
        public void Create_ThrowsInvalidOperationException_WhenPassedNullLuaTable()
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
            var luaTable = new Dictionary<object, object>();

            try
            {
                var result = CategoriesCreator.Create(luaTable, null);
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
        public void Create_CreatesADictionaryEntryForEachsnapshotSetting_WhenCategorySettingsArePassedAndHaveValidData(
                params string[] categoryNames)
        {
            var luaTable = new Dictionary<object, object>();
            var subTable = new Dictionary<object, object>()
            {
                { "test", new Dictionary<object, object>() { { 1, 1f } } }
            };
            var categorySettings = new CategorySetting[categoryNames.Length];
            for(int i = 0; i < categoryNames.Length; i++)
            {
                var categoryName = categoryNames[i];
                luaTable[categoryName] = subTable;
                categorySettings[i] = new CategorySetting(categoryName);
            }

            var result = CategoriesCreator.Create(luaTable, categorySettings);

            Assert.Equal(categoryNames.Length, result.Keys.Count);
        }
    }
}
