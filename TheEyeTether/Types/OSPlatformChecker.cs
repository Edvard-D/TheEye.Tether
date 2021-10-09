using System.Runtime.InteropServices;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Types
{
    public class OSPlatformChecker : IOSPlatformChecker
    {
        public bool IsOSPlatform(OSPlatform osPlatform)
        {
            return RuntimeInformation.IsOSPlatform(osPlatform);
        }
    }
}
