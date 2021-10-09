using System.Reflection;

namespace TheEyeTether.Interfaces
{
    public interface IAssemblyProvider
    {
        Assembly GetExecutingAssembly();
    }
}
