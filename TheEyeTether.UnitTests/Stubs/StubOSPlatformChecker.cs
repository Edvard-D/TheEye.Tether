using System.Runtime.InteropServices;
using TheEye.Tether.Interfaces;

namespace TheEye.Tether.UnitTests.Stubs
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
