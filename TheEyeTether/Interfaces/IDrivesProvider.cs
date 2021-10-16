using System.IO;

namespace TheEye.Tether.Interfaces
{
	public interface IDrivesProvider
	{
		DriveInfo[] GetDrives();
	}	
}
