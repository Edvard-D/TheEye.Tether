using System.Collections.Generic;
using System.Linq;

namespace TheEyeTether.Types
{
    public static class DataPointsCreator
    {
        public static Dictionary<string, List<DataPoint>> Create(Dictionary<object, object> luaTable)
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
                        (string)keyValuePair.Key);
            }

            return dataPoints;
        }

        private static List<DataPoint> ConvertTableToDataPoints(
                Dictionary<object, object> table,
                string typeName)
        {
            var subTypeNameTimestampPairs = GetSubTypeNameTimestampPairs(table);
            var dataPoints = new List<DataPoint>();

            while(subTypeNameTimestampPairs.Count > 0)
            {
                var subTypeNameTimestampPair = subTypeNameTimestampPairs[0];
                subTypeNameTimestampPairs.RemoveAt(0);
                var timestampRange = new TimestampRange(subTypeNameTimestampPair.Timestamp,
                        GetEndTimestamp(subTypeNameTimestampPairs));
                dataPoints.Add(new DataPoint(typeName, subTypeNameTimestampPair.SubTypeName,
                        timestampRange));
            }

            return dataPoints;
        }

        private static List<SubTypeNameTimestampPair> GetSubTypeNameTimestampPairs(
                Dictionary<object, object> table)
        {
            var subTypeNameTimestampPairs = new List<SubTypeNameTimestampPair>();

            /// Value is a timestamp
            if(table.ContainsKey(1))
            {
                subTypeNameTimestampPairs.AddRange(ConvertTableToSubTypeNameTimestampPairs(
                        null, table as Dictionary<object, object>));
            }
            /// Value is a table
            else
            {
                foreach(KeyValuePair<object, object> keyValuePair in table)
                {
                    subTypeNameTimestampPairs.AddRange(ConvertTableToSubTypeNameTimestampPairs(
                            (string)keyValuePair.Key,
                            keyValuePair.Value as Dictionary<object, object>));
                }
            }
            
            subTypeNameTimestampPairs = subTypeNameTimestampPairs.OrderBy(p => p.Timestamp).ToList();

            return subTypeNameTimestampPairs;
        }

        private static List<SubTypeNameTimestampPair> ConvertTableToSubTypeNameTimestampPairs(
                string subTypeName,
                Dictionary<object, object> timestamps)
        {
            var subTypeNameTimestampPairs = new List<SubTypeNameTimestampPair>();

            foreach(KeyValuePair<object, object> keyValuePair in timestamps)
            {
                subTypeNameTimestampPairs.Add(new SubTypeNameTimestampPair(subTypeName,
                        (float)keyValuePair.Value));
            }

            return subTypeNameTimestampPairs;
        }

        private static float GetEndTimestamp(List<SubTypeNameTimestampPair> subTypeNameTimestampPairs)
        {
            if(subTypeNameTimestampPairs.Count == 0)
            {
                return float.MaxValue;
            }

            return subTypeNameTimestampPairs[0].Timestamp;
        }


        private record SubTypeNameTimestampPair
        {
            public string SubTypeName;
            public float Timestamp;


            public SubTypeNameTimestampPair(string subTableName, float timestamp) =>
                    (SubTypeName, Timestamp) = (subTableName, timestamp);
        }
    }
}
