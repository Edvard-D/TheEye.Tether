using System.Collections.Generic;
using System.Linq;
using TheEye.Tether.Data;

namespace TheEye.Tether.Utilities.Hypotheses
{
	public static class DataPointsCreator
	{
		/// Expects luaTable's 2nd level objects to be Dictionary<object, object> values that represent
		/// a table for a data point type, defined in dataPointSettings. Each of those 2nd level tables
		/// will have 3rd level tables that represent either timestamps or 4th level sub type sub table of
		/// timestamps. The hierarchy can thus be:
		///
		/// luaTable > data point type > timestamps
		/// OR
		/// luaTable > data point type > sub type > timestamps
		/// 
		/// DataPointSetting will determine how the data is handled. If the EndMarker value is not null
		/// then it is expected that some of the sub type tables will contain timestamps that are only
		/// meant to be used as the TimestampRange.End value of corresponding sub types. As an example,
		/// take a sub type called "example_true" with a DataPointSetting.EndMarker value of "false". This
		/// indicates that the values in the sub type table "example_false" are meant to be used as the
		/// TimestampRange.End value for the DataPoint that gets created, while the timestamps in the
		/// "example_true" table are used as the TimestampRange.Start value.
		public static Dictionary<string, List<DataPoint>> Create(
				Dictionary<object, object> luaTable,
				Dictionary<string, DataPointSetting> dataPointSettings,
				Dictionary<string, CategorySetting> categorySettings = null)
		{
			var dataPoints = new Dictionary<string, List<DataPoint>>();
			foreach(KeyValuePair<object, object> keyValuePair in luaTable)
			{
				var subTable = keyValuePair.Value as Dictionary<object, object>;
				dataPoints[(string)keyValuePair.Key] = ConvertTableToDataPoints(subTable,
						(string)keyValuePair.Key, categorySettings, dataPointSettings);
			}

			return dataPoints;
		}

		private static List<DataPoint> ConvertTableToDataPoints(
				Dictionary<object, object> table,
				string typeName,
				Dictionary<string, CategorySetting> categorySettings,
				Dictionary<string, DataPointSetting> dataPointSettings)
		{
			var isSnapshotType = categorySettings != null
					&& categorySettings.Any(kvp => kvp.Value.SnapshotSettings.ContainsKey(typeName));
			var dataPointSetting = dataPointSettings[typeName];
			var timestampDatas = GetTimestampDatas(table, dataPointSetting);
			var dataPoints = new List<DataPoint>();
			
			while(timestampDatas.Count > 0)
			{
				var timestampData = timestampDatas[0];
				timestampDatas.RemoveAt(0);

				if(timestampData.EndMarkerSubTypeName != null
						&& timestampData.EndMarkerSubTypeName == timestampData.SubTypeName)
				{
					continue;
				}

				var timestampRange = new TimestampRange(timestampData.Timestamp,
						GetEndTimestamp(timestampData, timestampDatas, dataPointSetting, isSnapshotType));
				dataPoints.Add(new DataPoint(typeName, timestampData.SubTypeName, timestampRange));
			}

			return dataPoints;
		}

		private static List<TimestampData> GetTimestampDatas(
				Dictionary<object, object> table,
				DataPointSetting dataPointSetting)
		{
			var timestampDatas = new List<TimestampData>();

			/// Value is a timestamp
			if(table.ContainsKey(1L))
			{
				timestampDatas.AddRange(ConvertTableToSubTypeNameTimestampPairs(null, table,
						dataPointSetting));
			}
			/// Value is a table
			else
			{
				foreach(KeyValuePair<object, object> keyValuePair in table)
				{
					timestampDatas.AddRange(ConvertTableToSubTypeNameTimestampPairs(
							(string)keyValuePair.Key,
							keyValuePair.Value as Dictionary<object, object>, dataPointSetting));
				}
			}
			
			timestampDatas = timestampDatas.OrderBy(p => p.Timestamp).ToList();

			return timestampDatas;
		}

		private static List<TimestampData> ConvertTableToSubTypeNameTimestampPairs(
				string subTypeName,
				Dictionary<object, object> timestamps,
				DataPointSetting dataPointSetting)
		{
			var timestampDatas = new List<TimestampData>();
			string endMarkerSubTypeName = null;
			if(subTypeName != null && dataPointSetting.EndMarker != string.Empty)
			{
				var splitSubTypeName = subTypeName.Split("_");
				splitSubTypeName[dataPointSetting.EndMarkerPosition] = dataPointSetting.EndMarker;
				endMarkerSubTypeName = string.Join("_", splitSubTypeName);
			}

			foreach(KeyValuePair<object, object> keyValuePair in timestamps)
			{
				timestampDatas.Add(new TimestampData(subTypeName, endMarkerSubTypeName,
						(double)keyValuePair.Value));
			}

			return timestampDatas;
		}

		private static double GetEndTimestamp(
				TimestampData comparisonTimestampData,
				List<TimestampData> timestampDatas,
				DataPointSetting dataPointSetting,
				bool isSnapshotType)
		{
			if(isSnapshotType == true)
			{
				return comparisonTimestampData.Timestamp;
			}

			if(timestampDatas.Count == 0)
			{
				return double.MaxValue;
			}

			if(dataPointSetting.EndMarker == null || dataPointSetting.EndMarker == string.Empty)
			{
				return timestampDatas[0].Timestamp;
			}
			
			for(int i = 0; i < timestampDatas.Count; i++)
			{
				var timestampData = timestampDatas[i];

				if(timestampData.SubTypeName == comparisonTimestampData.EndMarkerSubTypeName
						&& timestampData.Timestamp >= comparisonTimestampData.Timestamp)
				{
					timestampDatas.RemoveAt(i);

					return timestampData.Timestamp;
				}
			}

			return double.MaxValue;
		}


		private record TimestampData
		{
			public string EndMarkerSubTypeName;
			public string SubTypeName;
			public double Timestamp;


			public TimestampData(string subTypeName, string endMarkerSubTypeName, double timestamp) => 
				(EndMarkerSubTypeName, SubTypeName, Timestamp) = (endMarkerSubTypeName, subTypeName, timestamp);
		}
	}
}
