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

            return dataPoints;
        }
    }
}
