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
                List<Snapshot> snapshotTypeSnapshots;
                var snapshotTypeLuaTable = luaTable[snapshotType.Name] as Dictionary<object, object>;

                /// Value is a timestamp
                if(snapshotTypeLuaTable.ContainsKey(0))
                {
                    snapshotTypeSnapshots = CreateSnapshotsForSubTable(snapshotType, luaTable,
                            snapshotType.Name, snapshotTypeLuaTable);
                }
                /// Value is a table
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

                    snapshot.AddDataPoint(dataPoints[0]);
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
                var value = keyValuePair.Value as Dictionary<object, object>;

                /// Value is a timestamp
                if(value == null)
                {
                    if(name == null)
                    {
                        name = type;
                    }

                    dataPoints.Add(new DataPoint(type, name, (float)keyValuePair.Value));
                }
            }

            return dataPoints;
        }
    }
}
