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
                List<Snapshot> snapshotTypeSnapshots;

                /// Table entries can either be table of floats or a table of tables. If this can
                /// be cast as object[] we know to handle it as a table of floats, otherwise we'll
                /// handle it as table of tables.
                if(luaTable[snapshotType.Name] as object[] != null)
                {
                    var snapshotTypeLuaTable = luaTable[snapshotType.Name] as object[];
                    snapshotTypeSnapshots = CreateSnapshotsFromTable(snapshotType.Name,
                            snapshotTypeLuaTable);
                }
                else
                {
                    snapshotTypeSnapshots = new List<Snapshot>();
                    var snapshotTypeLuaTable = luaTable[snapshotType.Name] as Dictionary<object, object>;

                    foreach(KeyValuePair<object, object> element in snapshotTypeLuaTable)
                    {
                        snapshotTypeSnapshots.AddRange(CreateSnapshotsFromTable((string)element.Key,
                                element.Value as object[]));
                    }
                }

                snapshots[snapshotType] = snapshotTypeSnapshots;
            }

            return snapshots;
        }

        private static List<Snapshot> CreateSnapshotsFromTable(
                string tableName,
                object[] table)
        {
            var snapshots = new List<Snapshot>();

            foreach(float timestamp in table)
            {
                snapshots.Add(new Snapshot(tableName, timestamp));
            }

            return snapshots;
        }
    }
}
