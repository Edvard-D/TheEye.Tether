using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Types
{
    public static class ProgramPathLocater
    {
        public static string LocateProgramPath(
                string programName,
                IFileSystem fileSystem,
                IDrivesGetter drivesGetter)
        {
            var searchPattern = "*" + programName;
            var files = new List<string>();

            foreach(DriveInfo driveInfo in drivesGetter.GetDrives())
            {
                files.AddRange(fileSystem.Directory.GetFiles(driveInfo.Name, searchPattern,
                        SearchOption.AllDirectories));
                
                if(files.Count > 0)
                {
                    break;
                }
            }

            return files[0];
        }
    }
}
