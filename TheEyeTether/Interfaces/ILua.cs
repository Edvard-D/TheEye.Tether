using System.Collections.Generic;
using NLua;

namespace TheEyeTether.Interfaces
{
    public interface ILua
    {
        Dictionary<object, object> ConvertTableHierarchyToDict(LuaTable luaTable)
        {
            var outputDict = new Dictionary<object, object>();

            foreach(KeyValuePair<object, object> keyValuePair in luaTable)
            {
                var valueTable = keyValuePair.Value as LuaTable;
                var valuesEnumerator = valueTable.Values.GetEnumerator();
                valuesEnumerator.MoveNext();

                if(valuesEnumerator.Current.GetType() == typeof(LuaTable))
                {
                    outputDict[keyValuePair.Key] = ConvertTableHierarchyToDict(valueTable);
                }
                else
                {
                    outputDict[keyValuePair.Key] = GetTableDict(valueTable);
                }
            }

            return outputDict;
        }
        bool DoesFileExist(string filePath);
        object[] DoFile(string filePath);
        LuaTable GetTable(string tableName);
        Dictionary<object, object> GetTableDict(LuaTable luaTable);
    }
}
