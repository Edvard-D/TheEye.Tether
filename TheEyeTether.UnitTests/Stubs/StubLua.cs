using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using NLua;
using TheEye.Tether.Interfaces;

namespace TheEye.Tether.UnitTests.Mocks
{
	public class StubLua : ILua
	{
		private Dictionary<string, string> _files;
		private Lua _lua;
		private string _tempFilePath;


		public StubLua(string projectDirectoryName, Dictionary<string, string> files)
		{
			_files = files;
			_lua = new Lua();
			
			/// Evidently cannot write directly to files in bin/net5.0/, so we need to create a
			/// different temp path that's based on this project's path.
			var entryAssemblyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			var entryAssemblyPathElements = entryAssemblyPath.Split(@"/\".ToCharArray());
			var tempFilePathElements = new List<string>();
			foreach(string element in entryAssemblyPathElements)
			{
				tempFilePathElements.Add(element);

				if(element == projectDirectoryName)
				{
					break;
				}
			}
			tempFilePathElements.Add("temp.lua");
			_tempFilePath = Path.Combine(tempFilePathElements.ToArray());
		}


		public bool DoesFileExist(string filePath)
		{
			return _files.ContainsKey(filePath);
		}

		public object[] DoFile(string filePath)
		{
			File.WriteAllText(_tempFilePath, _files[filePath], Encoding.ASCII);
			var result = _lua.DoFile(_tempFilePath);
			File.Delete(_tempFilePath);

			return result;
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
