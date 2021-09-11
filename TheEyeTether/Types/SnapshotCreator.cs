using System.Collections.Generic;

namespace TheEyeTether.Types
{
    public static class SnapshotCreator
    {
        public static List<Snapshot> Create(Dictionary<object, object> luaTable)
        {
            if(luaTable == null)
            {
                return null;
            }

            return new List<Snapshot>();
        }
    }
}
