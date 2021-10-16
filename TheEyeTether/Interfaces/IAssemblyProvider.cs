using System.Reflection;

namespace TheEye.Tether.Interfaces
{
	public interface IAssemblyProvider
	{
		Assembly GetExecutingAssembly();
	}
}
