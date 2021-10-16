using System.IO;
using TheEye.Tether.Interfaces;

namespace TheEye.Tether.Providers
{
	public class DrivesProvider : IDrivesProvider
	{
		public DriveInfo[] GetDrives()
		{
			return DriveInfo.GetDrives();
		}
	}
}
