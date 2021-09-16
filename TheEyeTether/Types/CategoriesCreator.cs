using System.Collections.Generic;

namespace TheEyeTether.Types
{
    public static class CategoriesCreator
    {
        public static Dictionary<CategorySetting, Category> Create(
                Dictionary<object, object> luaTable,
                CategorySetting[] categorySettings)
        {
            return new Dictionary<CategorySetting, Category>();
        }
    }
}
