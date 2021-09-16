using System.Collections.Generic;
using System.Linq;

namespace TheEyeTether.Types
{
    public static class SnapshotsCreator
    {
        public static Dictionary<CategorySetting, Dictionary<SnapshotSetting, List<Snapshot>>> Create(
                Dictionary<object, object> luaTable,
                Dictionary<string, CategorySetting> categorySettings,
                Dictionary<string, DataPointSetting> dataPointSettings)
        {
            if(luaTable == null)
            {
                throw new System.InvalidOperationException(string.Format("Argument {0} cannot be null.",
                        nameof(luaTable)));
            }

            if(categorySettings == null)
            {
                throw new System.InvalidOperationException(string.Format("Argument {0} cannot be null.",
                        nameof(categorySettings)));
            }
            
            var snapshots = new Dictionary<CategorySetting, Dictionary<SnapshotSetting, List<Snapshot>>>();

            foreach(KeyValuePair<string, CategorySetting> categoryKeyValuePair in categorySettings)
            {
                var categorySettingSnapshots = CreateSnapshotsForCategory(categoryKeyValuePair.Value, luaTable,
                        dataPointSettings);
                
                if(categorySettingSnapshots.Count == 0)
                {
                    continue;
                }

                snapshots[categoryKeyValuePair.Value] = categorySettingSnapshots;
            }

            return snapshots;
        }
        
        private static Dictionary<SnapshotSetting, List<Snapshot>> CreateSnapshotsForCategory(
                CategorySetting categorySetting,
                Dictionary<object, object> luaTable,
                Dictionary<string, DataPointSetting> dataPointSettings)
        {
            var categorySettingSnapshots = new Dictionary<SnapshotSetting, List<Snapshot>>();
            var dataPoints = DataPointsCreator.Create(luaTable, dataPointSettings, categorySetting);

            foreach(KeyValuePair<string, SnapshotSetting> settingKeyValuePair in
                    categorySetting.SnapshotSettings)
            {
                if(settingKeyValuePair.Value.DataPointTypeNames == null
                        || settingKeyValuePair.Value.DataPointTypeNames.Count() == 0)
                {
                    throw new System.InvalidOperationException(string.Format(
                            "{0} {1} does not have any {2} assigned.",
                            nameof(settingKeyValuePair), settingKeyValuePair.Key,
                            nameof(settingKeyValuePair.Value.DataPointTypeNames)));
                }

                if(!dataPoints.ContainsKey(settingKeyValuePair.Key))
                {
                    continue;
                }

                var snapshotSettingSnapshots = CreateSnapshotsForSnapshotSetting(settingKeyValuePair.Key,
                        settingKeyValuePair.Value, luaTable, dataPoints);

                if(snapshotSettingSnapshots.Count == 0)
                {
                    continue;
                }

                categorySettingSnapshots[settingKeyValuePair.Value] = snapshotSettingSnapshots;
            }

            return categorySettingSnapshots;
        }

        private static List<Snapshot> CreateSnapshotsForSnapshotSetting(
                string name,
                SnapshotSetting setting,
                Dictionary<object, object> luaTable,
                Dictionary<string, List<DataPoint>> dataPoints)
        {
            var snapshotSettingSnapshots = new List<Snapshot>();
            var snapshotSettingLuaTable = luaTable[name] as Dictionary<object, object>;
            var snapshotSettingLuaTableValues = new List<object>(snapshotSettingLuaTable.Values);

            foreach(DataPoint dataPoint in dataPoints[name])
            {
                var snapshot = CreateSnapshot(setting, dataPoint, dataPoints);

                if(snapshot == default(Snapshot))
                {
                    continue;
                }

                snapshotSettingSnapshots.Add(snapshot);
            }

            return snapshotSettingSnapshots;
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

                var validDataPoints = dataPoints[dataPointTypeName]
                        .Where(dp => dp.TimestampRange.Start <= snapshotDataPoint.TimestampRange.Start
                                && dp.TimestampRange.End > snapshotDataPoint.TimestampRange.End)
                        .ToList();

                snapshot.DataPoints.AddRange(validDataPoints);
            }

            if(snapshot.DataPoints.Count == 0)
            {
                return default(Snapshot);
            }

            return snapshot;
        }
    }
}
