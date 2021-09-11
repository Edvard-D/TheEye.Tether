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
            var luaTable = lua.GetTable(tableName);
            
            if(luaTable == null)
            {
                return null;
            }

            return lua.GetTableDict(luaTable);
        }
    }
}
