using System;
using System.Reflection;
using TheEye.Tether.Interfaces;

namespace TheEye.Tether.Providers
{
	public class AssemblyProvider : IAssemblyProvider
	{
		public Assembly GetAssemblyByName(string name)
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach(Assembly assembly in assemblies)
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
			return Assembly.GetExecutingAssembly();
		}
	}
}
