using System.Collections.Generic;
using System.IO;
using TheEyeTether.Interfaces;

namespace TheEyeTether.UnitTests.Stubs
{
    public class DrivesGetterStub : IDrivesGetter
    {
        private List<DriveInfo> _drives;

        
        public DrivesGetterStub(List<string> driveNames)
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