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
                snapshots[snapshotType] = new List<Snapshot>();
            }

            return snapshots;
        }
    }
}
