using System.Collections.Generic;

namespace TheEyeTether.Types
{
    public static class SnapshotsCreator
    {
        public static List<Snapshot> Create(
                Dictionary<object, object> luaTable,
                string[] snapshotTypes)
        {
            if(luaTable == null || snapshotTypes == null)
            {
                return null;
            }

            return new List<Snapshot>();
        }
    }
}
