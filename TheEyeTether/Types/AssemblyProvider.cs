using System.Reflection;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Types
{
    public class AssemblyProvider : IAssemblyProvider
    {
        public Assembly GetExecutingAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }
    }
}
