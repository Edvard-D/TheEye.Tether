using System.IO;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Providers
{
    public class DrivesProvider : IDrivesProvider
    {
        public DriveInfo[] GetDrives()
        {
            return DriveInfo.GetDrives();
        }
    }
}
