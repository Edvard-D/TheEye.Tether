using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace TheEyeTether.Types
{
    public static class SnapshotsCreator
    {
        public static Dictionary<SnapshotSetting, List<Snapshot>> Create(
                Dictionary<object, object> luaTable,
                SnapshotSetting[] snapshotSettings)
        {
            if(luaTable == null || snapshotSettings == null)
            {
                return null;
            }

            var snapshots = new Dictionary<SnapshotSetting, List<Snapshot>>();
            var dataPoints = DataPointsCreator.Create(luaTable);

            foreach(SnapshotSetting snapshotSetting in snapshotSettings)
            {
                if(snapshotSetting.DataPointTypeNames.Count() == 0)
                {
                    throw new System.InvalidOperationException(string.Format(
                            "snapshotSetting {0} does not have any DataPointTypeNames assigned.",
                            snapshotSetting.Name));
                }

                if(!dataPoints.ContainsKey(snapshotSetting.Name))
                {
                    continue;
                }

                var snapshotSettingSnapshots = new List<Snapshot>();
                var snapshotSettingLuaTable = luaTable[snapshotSetting.Name] as Dictionary<object, object>;
                var snapshotSettingLuaTableValues = new List<object>(snapshotSettingLuaTable.Values);

                foreach(DataPoint dataPoint in dataPoints[snapshotSetting.Name])
                {
                    var snapshot = CreateSnapshot(snapshotSetting, dataPoint, dataPoints);

                    if(snapshot == default(Snapshot))
                    {
                        continue;
                    }

                    snapshotSettingSnapshots.Add(snapshot);
                }

                if(snapshotSettingSnapshots.Count == 0)
                {
                    continue;
                }

                snapshots[snapshotSetting] = snapshotSettingSnapshots;
            }

            return snapshots;
        }
        
        private static Snapshot CreateSnapshot(
                SnapshotSetting snapshotSetting,
                DataPoint snapshotDataPoint,
                Dictionary<string, List<DataPoint>> dataPoints)
        {
            var snapshot = new Snapshot(snapshotDataPoint);
            
            foreach(string dataPointTypeName in snapshotSetting.DataPointTypeNames)
            {
                if(!dataPoints.ContainsKey(dataPointTypeName))
                {
                    continue;
                }

                var dataPoint = dataPoints[dataPointTypeName]
                        .Where(dp => dp.TimestampRange.Start <= snapshotDataPoint.TimestampRange.Start)
                        .MaxBy(dp => dp.TimestampRange.Start)
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
