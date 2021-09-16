using System.Collections.Generic;

namespace TheEyeTether.Types
{
    public static class CategoriesCreator
    {
        public static Dictionary<CategorySetting, List<Category>> Create(
                Dictionary<object, object> luaTable,
                CategorySetting[] categorySettings)
        {
            if(luaTable == null)
            {
                throw new System.InvalidOperationException(string.Format("Argument {0} cannot be null.",
                        nameof(luaTable)));
            }

            if(categorySettings == null)
            {
                throw new System.InvalidOperationException(string.Format("Argument {0} cannot be null.",
                        nameof(categorySettings)));
            }

            var categories = new Dictionary<CategorySetting, List<Category>>();
            foreach(CategorySetting categorySetting in categorySettings)
            {
                categories[categorySetting] = new List<Category>();
                categories[categorySetting].Add(new Category(string.Empty, new List<TimestampRange>()));
            }

            return categories;
        }
    }
}
