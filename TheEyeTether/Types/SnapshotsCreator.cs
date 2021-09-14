using System.Collections.Generic;
using System.Linq;

namespace TheEyeTether.Types
{
    public static class SnapshotsCreator
    {
        public static Dictionary<SnapshotSetting, List<Snapshot>> Create(
                Dictionary<object, object> luaTable,
                Dictionary<string, SnapshotSetting> snapshotSettings,
                Dictionary<string, DataPointSetting> dataPointSettings)
        {
            if(luaTable == null)
            {
                throw new System.InvalidOperationException(string.Format("Argument {0} cannot be null.",
                        nameof(luaTable)));
            }

            if(snapshotSettings == null)
            {
                throw new System.InvalidOperationException(string.Format("Argument {0} cannot be null.",
                        nameof(snapshotSettings)));
            }

            var snapshots = new Dictionary<SnapshotSetting, List<Snapshot>>();
            var dataPoints = DataPointsCreator.Create(luaTable, dataPointSettings, snapshotSettings);

            foreach(KeyValuePair<string, SnapshotSetting> keyValuePair in snapshotSettings)
            {
                if(keyValuePair.Value.DataPointTypeNames == null
                        || keyValuePair.Value.DataPointTypeNames.Count() == 0)
                {
                    throw new System.InvalidOperationException(string.Format(
                            "{0} {1} does not have any {2} assigned.",
                            nameof(keyValuePair), keyValuePair.Key,
                            nameof(keyValuePair.Value.DataPointTypeNames)));
                }

                if(!dataPoints.ContainsKey(keyValuePair.Key))
                {
                    continue;
                }

                var snapshotSettingSnapshots = new List<Snapshot>();
                var snapshotSettingLuaTable = luaTable[keyValuePair.Key] as Dictionary<object, object>;
                var snapshotSettingLuaTableValues = new List<object>(snapshotSettingLuaTable.Values);

                foreach(DataPoint dataPoint in dataPoints[keyValuePair.Key])
                {
                    var snapshot = CreateSnapshot(keyValuePair.Value, dataPoint, dataPoints);

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

                snapshots[keyValuePair.Value] = snapshotSettingSnapshots;
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
