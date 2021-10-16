using System.Reflection;
using TheEyeTether.Interfaces;

namespace TheEyeTether.UnitTests.Stubs
{
	public class StubAssemblyProvider : IAssemblyProvider
	{
		private Assembly _assembly;


		public StubAssemblyProvider(Assembly assembly)
		{
			_assembly = assembly;
		}


		public Assembly GetExecutingAssembly()
		{
			return _assembly;
		}
	}
}
