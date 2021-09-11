using System.Collections.Generic;
using TheEyeTether.Types;
using TheEyeTether.UnitTests.Mocks;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class LuaTableReaderTests
    {
        [Fact]
        public void Read_ReturnsObjectDictionary_WhenValidDataExists()
        {
            var filePath = "test.lua";
            var tableName = "test";
            var fileContent = tableName + " = { 0 }";
            var mockLua = new MockLua("TheEyeTether", new Dictionary<string, string>()
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
            var mockLua = new MockLua("TheEyeTether", new Dictionary<string, string>()
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
            var mockLua = new MockLua("TheEyeTether", new Dictionary<string, string>());
            
            var result = LuaTableReader.Read(filePath, tableName, mockLua);

            Assert.Null(result);
        }
    }
}
