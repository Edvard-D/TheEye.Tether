using System.Collections.Generic;
using TheEye.Tether.UnitTests.Mocks;
using TheEye.Tether.Utilities.General;
using Xunit;

namespace TheEye.Tether.UnitTests.Tests.Utilities.General
{
	public class LuaTableReaderTests
	{
		[Fact]
		public void Read_ReturnsObjectDictionary_WhenValidDataExists()
		{
			var filePath = "test.lua";
			var tableName = "test";
			var fileContent = tableName + " = { 0 }";
			var mockLua = new StubLua("TheEyeTether", new Dictionary<string, string>()
			{
				{ filePath, fileContent }
			});

			var result = LuaTableReader.Read(filePath, tableName, mockLua);

			Assert.IsType<Dictionary<object, object>>(result);
		}

		[Fact]
		public void Read_ReturnsNull_WhenValidDataDoesNotExist()
		{
			var filePath = "test.lua";
			var tableName = "test";
			var mockLua = new StubLua("TheEyeTether", new Dictionary<string, string>()
			{
				{ filePath, string.Empty }
			});

			var result = LuaTableReader.Read(filePath, tableName, mockLua);

			Assert.Null(result);
		}

		[Fact]
		public void Read_ReturnsNull_WhenFileDoesNotExist()
		{
			var filePath = "test.lua";
			var tableName = "test";
			var mockLua = new StubLua("TheEyeTether", new Dictionary<string, string>());
			
			var result = LuaTableReader.Read(filePath, tableName, mockLua);

			Assert.Null(result);
		}
	}
}
