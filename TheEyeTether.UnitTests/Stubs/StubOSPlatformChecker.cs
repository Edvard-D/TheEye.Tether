using System.Runtime.InteropServices;
using TheEyeTether.Interfaces;

namespace TheEyeTether.UnitTests.Stubs
{
    public class StubOSPlatformChecker : IOSPlatformChecker
    {
        private OSPlatform _currentOSPlatform;


        public StubOSPlatformChecker(OSPlatform currentOSPlatform)
        {
            _currentOSPlatform = currentOSPlatform;
        }


        public bool IsOSPlatform(OSPlatform osPlatform)
        {
            return osPlatform == _currentOSPlatform;
        }
    }
}
