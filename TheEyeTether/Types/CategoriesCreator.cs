using System.Collections.Generic;

namespace TheEyeTether.Types
{
    public static class CategoriesCreator
    {
        public static Dictionary<CategorySetting, Category> Create(
                Dictionary<object, object> luaTable,
                CategorySetting[] categorySettings)
        {
            if(luaTable == null)
            {
                throw new System.InvalidOperationException(string.Format("Argument {0} cannot be null.",
                        nameof(luaTable)));
            }

            return new Dictionary<CategorySetting, Category>();
        }
    }
}
