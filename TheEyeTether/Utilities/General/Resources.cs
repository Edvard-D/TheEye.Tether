using System.IO;
using System.Linq;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Utilities.General
{
    public static class Resources
    {
        public static string ReadTextResource(
                string resourceName,
                IAssemblyProvider assemblyProvider)
        {
            var assembly = assemblyProvider.GetExecutingAssembly();
            var resourcePath = assembly.GetManifestResourceNames().Single(s => s.EndsWith(resourceName));

            using(Stream stream = assembly.GetManifestResourceStream(resourcePath))
            using(StreamReader streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
