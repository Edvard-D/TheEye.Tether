using System;
using System.IO;
using System.Reflection;
using System.Text;
using Moq;
using TheEyeTether.UnitTests.Stubs;
using TheEyeTether.Utilities.General;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Utilities.General
{
    public class ResourcesTests
    {
        [Fact]
        public void ReadTextResource_ReturnsString_WhenCalled()
        {
            var resourceName = "Path.txt";
            var resourcePath = "Test." + resourceName;
            var resourceText = "testText";
            var manifestResourceNames = new string[] { resourcePath };
            var mockAssembly = new Mock<Assembly>();
            mockAssembly.Setup(a => a.GetManifestResourceNames()).Returns(manifestResourceNames);
            mockAssembly.Setup(a => a.GetManifestResourceStream(resourcePath)).Returns(
                    new MemoryStream(Encoding.UTF8.GetBytes(resourceText)));
            var stubAssemblyProvider = new StubAssemblyProvider(mockAssembly.Object);

            var result = Resources.ReadTextResource(resourceName, stubAssemblyProvider);

            Assert.IsType<string>(result);
        }

        [Fact]
        public void ReadTextResource_ReturnsResourceValue_WhenCalledWithValidResourceName()
        {
            var resourceName = "Path.txt";
            var resourcePath = "Test." + resourceName;
            var resourceText = "testText";
            var manifestResourceNames = new string[] { resourcePath };
            var mockAssembly = new Mock<Assembly>();
            mockAssembly.Setup(a => a.GetManifestResourceNames()).Returns(manifestResourceNames);
            mockAssembly.Setup(a => a.GetManifestResourceStream(resourcePath)).Returns(
                    new MemoryStream(Encoding.UTF8.GetBytes(resourceText)));
            var stubAssemblyProvider = new StubAssemblyProvider(mockAssembly.Object);

            var result = Resources.ReadTextResource(resourceName, stubAssemblyProvider);

            Assert.Equal(resourceText, result);
        }

        [Fact]
        public void ReadTextResource_ThrowsInvalidOperationException_WhenResourceDoesNotExist()
        {
            var resourceName = "Path.txt";
            var manifestResourceNames = new string[] {};
            var mockAssembly = new Mock<Assembly>();
            mockAssembly.Setup(a => a.GetManifestResourceNames()).Returns(manifestResourceNames);
            var stubAssemblyProvider = new StubAssemblyProvider(mockAssembly.Object);
            
            try
            {
                var result = Resources.ReadTextResource(resourceName, stubAssemblyProvider);
                Assert.True(false);
            }
            catch(Exception ex)
            {
                Assert.IsType<System.InvalidOperationException>(ex);
            }
        }
    }
}
