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
    }
}
