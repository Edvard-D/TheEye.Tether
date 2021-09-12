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
                string type,
                string name = null)
        {
            var dataPoints = new List<DataPoint>();

            foreach(KeyValuePair<object, object> keyValuePair in table)
            {
                /// Value is a timestamp
                if(keyValuePair.Value.GetType() == typeof(float))
                {
                    if(name == null)
                    {
                        name = type;
                    }

                    dataPoints.Add(new DataPoint(type, name, (float)keyValuePair.Value));
                }
                /// Value is a table
                else
                {
                    var subTable = keyValuePair.Value as Dictionary<object, object>;
                    dataPoints.AddRange(ConvertTableToDataPoints(subTable, type,
                            (string)keyValuePair.Key));
                }
            }

            return dataPoints;
        }
    }
}
