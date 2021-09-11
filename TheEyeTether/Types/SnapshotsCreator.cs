using System.Collections.Generic;

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
                var snapshotTypeSnapshots = new List<Snapshot>();
                snapshots[snapshotType] = snapshotTypeSnapshots;

                var snapshotTypeLuaTable = luaTable[snapshotType.Name] as float[];
                foreach(float timestamp in snapshotTypeLuaTable)
                {
                    snapshotTypeSnapshots.Add(new Snapshot(timestamp));
                }
            }

            return snapshots;
        }
    }
}
