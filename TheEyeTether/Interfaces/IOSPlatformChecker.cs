using System.Runtime.InteropServices;

namespace TheEyeTether.Interfaces
{
    public interface IOSPlatformChecker
    {
        bool IsOSPlatform(OSPlatform osPlatform);
    }
}
