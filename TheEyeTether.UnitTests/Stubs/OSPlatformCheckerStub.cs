using System.Runtime.InteropServices;
using TheEyeTether.Interfaces;

namespace TheEyeTether.UnitTests.Stubs
{
    public class OSPlatformCheckerStub : IOSPlatformChecker
    {
        private OSPlatform _currentOSPlatform;


        public OSPlatformCheckerStub(OSPlatform currentOSPlatform)
        {
            _currentOSPlatform = currentOSPlatform;
        }


        public bool IsOSPlatform(OSPlatform osPlatform)
        {
            return osPlatform == _currentOSPlatform;
        }
    }
}
