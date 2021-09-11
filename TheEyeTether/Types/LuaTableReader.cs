using System.Collections.Generic;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Types
{
    public static class LuaTableReader
    {
        public static Dictionary<object, object> Read(
                string filePath,
                string tableName,
                ILua lua)
        {
            lua.DoFile(filePath);

            return lua.GetTableDict(lua.GetTable(tableName));
        }
    }
}
