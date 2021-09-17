using System.Collections.Generic;

namespace TheEyeTether.Types
{
    public static class CategoriesCreator
    {
        public static Dictionary<CategorySetting, List<Category>> Create(
                Dictionary<string, List<DataPoint>> dataPoints,
                Dictionary<string, CategorySetting> categorySettings)
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
            foreach(KeyValuePair<string, CategorySetting> keyValuePair in categorySettings)
            {
                if(!dataPoints.ContainsKey(keyValuePair.Value.Name))
                {
                    continue;
                }

                categories[keyValuePair.Value] = CreateCategoriesForCategorySetting(keyValuePair.Value,
                        dataPoints);
            }

            return categories;
        }

        private static List<Category> CreateCategoriesForCategorySetting(
                CategorySetting categorySetting,
                Dictionary<string, List<DataPoint>> dataPoints)
        {
            var categorySettingCategories = new List<Category>();
            var categorySubTypeIndexes = new Dictionary<string, int>();

            foreach(DataPoint dataPoint in dataPoints[categorySetting.Name])
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

            return categorySettingCategories;
        }
    }
}
