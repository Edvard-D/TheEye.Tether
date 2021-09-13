using System.Collections.Generic;
using System.Linq;

namespace TheEyeTether.Types
{
    public static class DataPointsCreator
    {
        public static Dictionary<string, List<DataPoint>> Create(
                Dictionary<object, object> luaTable,
                Dictionary<string, DataPointSetting> dataPointSettings)
        {
            if(luaTable == null)
            {
                return null;
            }
            
            var dataPoints = new Dictionary<string, List<DataPoint>>();
            foreach(KeyValuePair<object, object> keyValuePair in luaTable)
            {
                var subTable = keyValuePair.Value as Dictionary<object, object>;
                dataPoints[(string)keyValuePair.Key] = ConvertTableToDataPoints(subTable,
                        (string)keyValuePair.Key, dataPointSettings);
            }

            return dataPoints;
        }

        private static List<DataPoint> ConvertTableToDataPoints(
                Dictionary<object, object> table,
                string typeName,
                Dictionary<string, DataPointSetting> dataPointSettings)
        {
            var dataPointSetting = dataPointSettings[typeName];
            var timestampDatas = GetTimestampDatas(table, dataPointSetting);
            var dataPoints = new List<DataPoint>();
            
            while(timestampDatas.Count > 0)
            {
                var timestampData = timestampDatas[0];
                timestampDatas.RemoveAt(0);
                var timestampRange = new TimestampRange(timestampData.Timestamp,
                        GetEndTimestamp(timestampData, timestampDatas, dataPointSetting));
                dataPoints.Add(new DataPoint(typeName, timestampData.SubTypeName,
                        timestampRange));
            }

            return dataPoints;
        }

        private static List<TimestampData> GetTimestampDatas(
                Dictionary<object, object> table,
                DataPointSetting dataPointSetting)
        {
            var timestampDatas = new List<TimestampData>();

            /// Value is a timestamp
            if(table.ContainsKey(1))
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
            if(subTypeName != null && dataPointSetting.EndMarker != null)
            {
                var splitSubTypeName = subTypeName.Split("_");
                splitSubTypeName[dataPointSetting.EndMarkerPosition] = dataPointSetting.EndMarker;
                endMarkerSubTypeName = string.Join("_", splitSubTypeName);
            }

            foreach(KeyValuePair<object, object> keyValuePair in timestamps)
            {
                timestampDatas.Add(new TimestampData(subTypeName, endMarkerSubTypeName,
                        (float)keyValuePair.Value));
            }

            return timestampDatas;
        }

        private static float GetEndTimestamp(
                TimestampData comparisonTimestampData,
                List<TimestampData> timestampDatas,
                DataPointSetting dataPointSetting)
        {
            if(timestampDatas.Count == 0)
            {
                return float.MaxValue;
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

            return float.MaxValue;
        }


        private record TimestampData
        {
            public string EndMarkerSubTypeName;
            public string SubTypeName;
            public float Timestamp;


            public TimestampData(string subTypeName, string endMarkerSubTypeName, float timestamp) => 
                (EndMarkerSubTypeName, SubTypeName, Timestamp) = (endMarkerSubTypeName, subTypeName, timestamp);
        }
    }
}
