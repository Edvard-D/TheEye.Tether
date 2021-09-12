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
            var dataPoints = DataPointsCreator.Create(luaTable);

            foreach(SnapshotType snapshotType in snapshotTypes)
            {
                if(snapshotType.DataPointTypeNames.Count() == 0)
                {
                    throw new System.InvalidOperationException(string.Format(
                            "SnapshotType {0} does not have any DataPointTypeNames assigned.",
                            snapshotType.Name));
                }

                if(!dataPoints.ContainsKey(snapshotType.Name))
                {
                    continue;
                }

                var snapshotTypeSnapshots = new List<Snapshot>();
                var snapshotTypeLuaTable = luaTable[snapshotType.Name] as Dictionary<object, object>;
                var snapshotTypeLuaTableValues = new List<object>(snapshotTypeLuaTable.Values);

                foreach(DataPoint dataPoint in dataPoints[snapshotType.Name])
                {
                    var snapshot = CreateSnapshot(snapshotType, dataPoint, dataPoints);

                    if(snapshot == default(Snapshot))
                    {
                        continue;
                    }

                    snapshotTypeSnapshots.Add(snapshot);
                }

                if(snapshotTypeSnapshots.Count == 0)
                {
                    continue;
                }

                snapshots[snapshotType] = snapshotTypeSnapshots;
            }

            return snapshots;
        }
        
        private static Snapshot CreateSnapshot(
                SnapshotType snapshotType,
                DataPoint snapshotDataPoint,
                Dictionary<string, List<DataPoint>> dataPoints)
        {
            var snapshot = new Snapshot(snapshotDataPoint);
            
            foreach(string dataPointTypeName in snapshotType.DataPointTypeNames)
            {
                if(!dataPoints.ContainsKey(dataPointTypeName))
                {
                    continue;
                }

                var dataPoint = dataPoints[dataPointTypeName]
                        .Where(dp => dp.Timestamp <= snapshotDataPoint.Timestamp)
                        .MaxBy(dp => dp.Timestamp)
                        .FirstOrDefault();

                if(dataPoint != default(DataPoint))
                {
                    snapshot.DataPoints.Add(dataPoint);
                }
            }

            if(snapshot.DataPoints.Count == 0)
            {
                return default(Snapshot);
            }

            return snapshot;
        }
    }
}
