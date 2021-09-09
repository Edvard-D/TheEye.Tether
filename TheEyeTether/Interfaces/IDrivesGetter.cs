using System.IO;

namespace TheEyeTether.Interfaces
{
    public interface IDrivesGetter
    {
        DriveInfo[] GetDrives();
    }    
}
