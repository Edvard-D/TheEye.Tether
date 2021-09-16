using System.Collections.Generic;

namespace TheEyeTether.Types
{
    public static class CategoriesCreator
    {
        public static Dictionary<CategorySetting, List<Category>> Create(
                Dictionary<string, List<DataPoint>> dataPoints,
                CategorySetting[] categorySettings)
        {
            if(dataPoints == null)
            {
                throw new System.InvalidOperationException(string.Format("Argument {0} cannot be null.",
                        nameof(dataPoints)));
            }

            if(categorySettings == null)
            {
                throw new System.InvalidOperationException(string.Format("Argument {0} cannot be null.",
                        nameof(categorySettings)));
            }

            var categories = new Dictionary<CategorySetting, List<Category>>();
            foreach(CategorySetting categorySetting in categorySettings)
            {
                var categoryDataPoints = dataPoints[categorySetting.Name];
                var categorySettingCategories = new List<Category>();
                var categorySubTypeIndexes = new Dictionary<string, int>();

                foreach(DataPoint dataPoint in categoryDataPoints)
                {
                    if(!categorySubTypeIndexes.ContainsKey(dataPoint.SubTypeName))
                    {
                        categorySettingCategories.Add(new Category(dataPoint.SubTypeName,
                                new List<TimestampRange>()));
                        categorySubTypeIndexes[dataPoint.SubTypeName] = categorySettingCategories.Count - 1;
                    }

                    categorySettingCategories[categorySubTypeIndexes[dataPoint.SubTypeName]]
                            .ActiveTimePeriods.Add(dataPoint.TimestampRange);
                }
                
                categories[categorySetting] = categorySettingCategories;
            }

            return categories;
        }
    }
}
