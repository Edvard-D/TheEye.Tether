using System.Reflection;
using TheEye.Tether.Interfaces;

namespace TheEye.Tether.Providers
{
	public class AssemblyProvider : IAssemblyProvider
	{
		public Assembly GetExecutingAssembly()
		{
			return Assembly.GetExecutingAssembly();
		}
	}
}
