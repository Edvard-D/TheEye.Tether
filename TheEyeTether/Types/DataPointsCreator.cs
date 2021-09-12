using System.Collections.Generic;

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
                string type)
        {
            var dataPoints = new List<DataPoint>();

            foreach(KeyValuePair<object, object> keyValuePair in table)
            {
                dataPoints.Add(new DataPoint(type, type, (float)keyValuePair.Value));
            }

            return dataPoints;
        }
    }
}
