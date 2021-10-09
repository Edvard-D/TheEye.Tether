using System.Collections.Generic;
using System.IO;
using TheEyeTether.Interfaces;

namespace TheEyeTether.UnitTests.Stubs
{
    public class StubDrivesProvider : IDrivesProvider
    {
        private List<DriveInfo> _drives;

        
        public StubDrivesProvider(List<string> driveNames)
        {
            var drives = new List<DriveInfo>();

            foreach(string driveName in driveNames)
            {
                drives.Add(new DriveInfo(driveName));
            }

            _drives = drives;
        }

        public DriveInfo[] GetDrives()
        {
            return _drives.ToArray();
        }
    }
}