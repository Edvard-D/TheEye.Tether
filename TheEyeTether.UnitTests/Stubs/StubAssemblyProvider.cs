using System.Collections.Generic;
using System.Reflection;
using TheEye.Tether.Interfaces;

namespace TheEye.Tether.UnitTests.Stubs
{
	public class StubAssemblyProvider : IAssemblyProvider
	{
		private List<Assembly> _assemblies;


		public StubAssemblyProvider(List<Assembly> assemblies)
		{
			_assemblies = assemblies;
		}

		public Assembly GetAssemblyByName(string name)
		{
			foreach(Assembly assembly in _assemblies)
			{
				var assemblyName = assembly.GetName().Name.ToString().Split(new char[] { '.' })[0];

				if(assemblyName == name)
				{
					return assembly;
				}
			}

			return null;
		}

		public Assembly GetExecutingAssembly()
		{
			return _assemblies[0];
		}
	}
}
