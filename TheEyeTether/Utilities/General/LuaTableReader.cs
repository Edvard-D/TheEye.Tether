using System.Collections.Generic;
using TheEye.Tether.Interfaces;

namespace TheEye.Tether.Utilities.General
{
	public static class LuaTableReader
	{
		public static Dictionary<object, object> Read(
				string filePath,
				string tableName,
				ILua lua)
		{
			if(!lua.DoesFileExist(filePath))
			{
				return null;
			}

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
