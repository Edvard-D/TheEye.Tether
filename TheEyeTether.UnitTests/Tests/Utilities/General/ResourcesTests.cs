using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Moq;
using TheEye.Tether.UnitTests.Stubs;
using TheEye.Tether.Utilities.General;
using Xunit;

namespace TheEye.Tether.UnitTests.Tests.Utilities.General
{
	public class ResourcesTests
	{
		[Fact]
		public void ReadTextResource_ReturnsString_WhenCalled()
		{
			var resourceName = "Path.txt";
			var resourcePath = "Test." + resourceName;
			var resourceText = "testText";
			var assemblyName = "assemblyName";
			var manifestResourceNames = new string[] { resourcePath };
			var mockAssembly = new Mock<Assembly>();
			mockAssembly.Setup(a => a.GetName()).Returns(new AssemblyName(assemblyName));
			mockAssembly.Setup(a => a.GetManifestResourceNames()).Returns(manifestResourceNames);
			mockAssembly.Setup(a => a.GetManifestResourceStream(resourcePath)).Returns(
					new MemoryStream(Encoding.UTF8.GetBytes(resourceText)));
			var assemblies = new List<Assembly>() { mockAssembly.Object };
			var stubAssemblyProvider = new StubAssemblyProvider(assemblies);

			var result = Resources.ReadTextResource(assemblyName, resourceName, stubAssemblyProvider);

			Assert.IsType<string>(result);
		}

		[Fact]
		public void ReadTextResource_ReturnsResourceValue_WhenCalledWithValidResourceName()
		{
			var resourceName = "Path.txt";
			var resourcePath = "Test." + resourceName;
			var resourceText = "testText";
			var assemblyName = "assemblyName";
			var manifestResourceNames = new string[] { resourcePath };
			var mockAssembly = new Mock<Assembly>();
			mockAssembly.Setup(a => a.GetName()).Returns(new AssemblyName(assemblyName));
			mockAssembly.Setup(a => a.GetManifestResourceNames()).Returns(manifestResourceNames);
			mockAssembly.Setup(a => a.GetManifestResourceStream(resourcePath)).Returns(
					new MemoryStream(Encoding.UTF8.GetBytes(resourceText)));
			var assemblies = new List<Assembly>() { mockAssembly.Object };
			var stubAssemblyProvider = new StubAssemblyProvider(assemblies);

			var result = Resources.ReadTextResource(assemblyName, resourceName, stubAssemblyProvider);

			Assert.Equal(resourceText, result);
		}

		[Fact]
		public void ReadTextResource_ThrowsInvalidOperationException_WhenResourceDoesNotExist()
		{
			var resourceName = "Path.txt";
			var assemblyName = "assemblyName";
			var manifestResourceNames = new string[] {};
			var mockAssembly = new Mock<Assembly>();
			mockAssembly.Setup(a => a.GetName()).Returns(new AssemblyName(assemblyName));
			mockAssembly.Setup(a => a.GetManifestResourceNames()).Returns(manifestResourceNames);
			var assemblies = new List<Assembly>() { mockAssembly.Object };
			var stubAssemblyProvider = new StubAssemblyProvider(assemblies);
			
			try
			{
				var result = Resources.ReadTextResource(assemblyName, resourceName, stubAssemblyProvider);
				Assert.True(false);
			}
			catch(Exception ex)
			{
				Assert.IsType<System.InvalidOperationException>(ex);
			}
		}

		[Fact]
		public void ReadTextResource_LoadsFilesFromTheCorrectAssembly_WhenCalled()
		{
			var resourceName = "Path.txt";
			var resourcePath = "Test." + resourceName;
			var resourceText1 = "testText1";
			var resourceText2 = "testText2";
			var assemblyName1 = "assemblyName1";
			var assemblyName2 = "assemblyName2";
			var manifestResourceNames = new string[] { resourcePath };
			var mockAssembly1 = new Mock<Assembly>();
			mockAssembly1.Setup(a => a.GetName()).Returns(new AssemblyName(assemblyName1));
			mockAssembly1.Setup(a => a.GetManifestResourceNames()).Returns(manifestResourceNames);
			mockAssembly1.Setup(a => a.GetManifestResourceStream(resourcePath)).Returns(
					new MemoryStream(Encoding.UTF8.GetBytes(resourceText1)));
			var mockAssembly2 = new Mock<Assembly>();
			mockAssembly2.Setup(a => a.GetName()).Returns(new AssemblyName(assemblyName2));
			mockAssembly2.Setup(a => a.GetManifestResourceNames()).Returns(manifestResourceNames);
			mockAssembly2.Setup(a => a.GetManifestResourceStream(resourcePath)).Returns(
					new MemoryStream(Encoding.UTF8.GetBytes(resourceText2)));
			var assemblies = new List<Assembly>() { mockAssembly1.Object, mockAssembly2.Object };
			var stubAssemblyProvider = new StubAssemblyProvider(assemblies);

			var result = Resources.ReadTextResource(assemblyName1, resourceName, stubAssemblyProvider);

			Assert.Equal(result, resourceText1);
		}
	}
}
