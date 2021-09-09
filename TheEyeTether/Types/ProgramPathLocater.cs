using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Types
{
    public static class ProgramPathLocater
    {
        private static Dictionary<string, string> _savedProgramPathPairs = new Dictionary<string, string>();


        public static Dictionary<string, string> SavedProgramPathPairs { get { return _savedProgramPathPairs;  } }


        public static string LocateProgramPath(
                string programName,
                IFileSystem fileSystem,
                IDrivesGetter drivesGetter)
        {
            if(_savedProgramPathPairs.ContainsKey(programName) == true
                    && fileSystem.File.Exists(_savedProgramPathPairs[programName]) == true)
            {
                return _savedProgramPathPairs[programName];
            }

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
