using TheEyeTether.Types;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class TimestampRangeTests
    {
        [Fact]
        public void Equals_ReturnsTrue_WhenAllValuesAreEqual()
        {
            var value1 = new TimestampRange(0, 0);
            var value2 = new TimestampRange(0, 0);

            var result = value1.Equals(value2);

            Assert.True(result);
        }

        [Fact]
        public void Equals_ReturnsFalse_WhenAllValuesAreNotEqual()
        {
            var value1 = new TimestampRange(0, 0);
            var value2 = new TimestampRange(1, 1);

            var result = value1.Equals(value2);

            Assert.False(result);
        }

        [Fact]
        public void Equals_ReturnsFalse_WhenStartValueIsNotEqual()
        {
            var value1 = new TimestampRange(0, 0);
            var value2 = new TimestampRange(1, 0);

            var result = value1.Equals(value2);

            Assert.False(result);
        }

        [Fact]
        public void Equals_ReturnsFalse_WhenEndValueIsNotEqual()
        {
            var value1 = new TimestampRange(0, 0);
            var value2 = new TimestampRange(0, 1);

            var result = value1.Equals(value2);

            Assert.False(result);
        }
    }
}
