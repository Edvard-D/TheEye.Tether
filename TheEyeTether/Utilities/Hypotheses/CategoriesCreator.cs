using System.Collections.Generic;
using TheEye.Tether.Data;

namespace TheEye.Tether.Utilities.Hypotheses
{
	public static class CategoriesCreator
	{
		public static Dictionary<CategorySetting, List<Category>> Create(
				Dictionary<string, List<DataPoint>> dataPoints,
				Dictionary<string, CategorySetting> categorySettings)
		{
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
					categorySettingCategories.Add(new Category(dataPoint.SubTypeName, categorySetting,
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
