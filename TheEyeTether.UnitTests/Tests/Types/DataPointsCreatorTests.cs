using System.Collections.Generic;
using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class DataPointsCreatorTests
    {
        [Fact]
        public void Create_ReturnsDictionaryOfListsOfDataPoints_WhenPassedValidLuaTable()
        {
            var luaTable = new Dictionary<object, object>()
            {
                { "test", new Dictionary<object, object>() { { 1, 1f } } }
            };

            var result = DataPointsCreator.Create(luaTable);

            Assert.IsType<Dictionary<string, List<DataPoint>>>(result);
        }

        [Fact]
        public void Create_ReturnsNull_WhenPassedNullLuaTable()
        {
            var result = DataPointsCreator.Create(null);

            Assert.Null(result);
        }
    }
}
