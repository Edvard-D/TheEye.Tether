using System.Reflection;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Providers
{
    public class AssemblyProvider : IAssemblyProvider
    {
        public Assembly GetExecutingAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }
    }
}
