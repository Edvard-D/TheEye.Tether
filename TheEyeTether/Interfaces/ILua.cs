using System.Collections.Generic;
using NLua;

namespace TheEyeTether.Interfaces
{
    public interface ILua
    {
        bool DoesFileExist(string filePath);
        object[] DoFile(string filePath);
        LuaTable GetTable(string tableName);
        Dictionary<object, object> GetTableDict(LuaTable luaTable);
    }
}
