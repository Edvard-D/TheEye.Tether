using System.Collections.Generic;
using System.IO.Abstractions;
using NLua;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Types
{
    public class Lua : ILua
    {
        private IFileSystem _fileSystem;
        private NLua.Lua _lua;


        public Lua(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _lua = new NLua.Lua();
        }


        public bool DoesFileExist(string filePath)
        {
            return _fileSystem.File.Exists(filePath);
        }

        public object[] DoFile(string filePath)
        {
            return _lua.DoFile(filePath);
        }

        public LuaTable GetTable(string tableName)
        {
            return _lua.GetTable(tableName);
        }

        public Dictionary<object, object> GetTableDict(LuaTable luaTable)
        {
            return _lua.GetTableDict(luaTable);
        }
    }
}
