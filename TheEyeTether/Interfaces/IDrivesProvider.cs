using System.IO;

namespace TheEyeTether.Interfaces
{
	public interface IDrivesProvider
	{
		DriveInfo[] GetDrives();
	}	
}
