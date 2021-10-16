using System.Runtime.InteropServices;
using TheEye.Tether.Interfaces;

namespace TheEye.Tether.Types
{
	public class OSPlatformChecker : IOSPlatformChecker
	{
		public bool IsOSPlatform(OSPlatform osPlatform)
		{
			return RuntimeInformation.IsOSPlatform(osPlatform);
		}
	}
}
