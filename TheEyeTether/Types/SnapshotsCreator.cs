using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace TheEyeTether.Types
{
    public static class SnapshotsCreator
    {
        public static Dictionary<SnapshotType, List<Snapshot>> Create(
                Dictionary<object, object> luaTable,
                SnapshotType[] snapshotTypes)
        {
            if(luaTable == null || snapshotTypes == null)
            {
                return null;
            }

            var snapshots = new Dictionary<SnapshotType, List<Snapshot>>();

            foreach(SnapshotType snapshotType in snapshotTypes)
            {
                if(!luaTable.ContainsKey(snapshotType))
                {
                    continue;
                }

                List<Snapshot> snapshotTypeSnapshots;
                var snapshotTypeLuaTable = luaTable[snapshotType.Name] as Dictionary<object, object>;
                var snapshotTypeLuaTableValues = new List<object>(snapshotTypeLuaTable.Values);

                /// Value is a timestamp table
                if(snapshotTypeLuaTableValues[0].GetType() == typeof(float))
                {
                    snapshotTypeSnapshots = CreateSnapshotsForSubTable(snapshotType, luaTable,
                            snapshotType.Name, snapshotTypeLuaTable);
                }
                /// Value is a sub table
                else
                {
                    snapshotTypeSnapshots = new List<Snapshot>();
                    
                    foreach(KeyValuePair<object, object> keyValuePair in snapshotTypeLuaTable)
                    {
                        snapshotTypeSnapshots.AddRange(CreateSnapshotsForSubTable(snapshotType,
                                luaTable, (string)keyValuePair.Key,
                                keyValuePair.Value as Dictionary<object, object>));
                    }
                }

                snapshots[snapshotType] = snapshotTypeSnapshots;
            }

            return snapshots;
        }

        private static List<Snapshot> CreateSnapshotsForSubTable(
                SnapshotType snapshotType,
                Dictionary<object, object> fullTable,
                string tableName,
                Dictionary<object, object> subTable)
        {
            var snapshots = new List<Snapshot>();

            foreach(KeyValuePair<object, object> keyValuePair in subTable)
            {
                var snapshot = new Snapshot(tableName, (float)keyValuePair.Value);
                
                foreach(string dataPointTypeName in snapshotType.DataPointTypeNames)
                {
                    var dataPointTable = fullTable[dataPointTypeName] as Dictionary<object, object>;
                    var dataPoints = ConvertTableToDataPoints(dataPointTable, dataPointTypeName);
                    var dataPoint = dataPoints
                            .Where(dp => dp.Timestamp <= snapshot.Timestamp)
                            .MaxBy(dp => dp.Timestamp)
                            .FirstOrDefault();

                    if(dataPoint != default(DataPoint))
                    {
                        snapshot.AddDataPoint(dataPoint);
                    }
                }

                snapshots.Add(snapshot);
            }

            return snapshots;
        }

        private static List<DataPoint> ConvertTableToDataPoints(
                Dictionary<object, object> table,
                string type,
                string name = null)
        {
            var dataPoints = new List<DataPoint>();

            foreach(KeyValuePair<object, object> keyValuePair in table)
            {
                var subTable = keyValuePair.Value as Dictionary<object, object>;

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
                    dataPoints.AddRange(ConvertTableToDataPoints(subTable, type,
                            (string)keyValuePair.Key));
                }
            }

            return dataPoints;
        }
    }
}
