using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Types
{
    public static class ProgramPathLocater
    {
        private const string MacOSProgramEnding = ".app";
        private const string WindowsProgramEnding = ".exe";


        private static Dictionary<string, string> _savedProgramPathPairs = new Dictionary<string, string>();


        public static Dictionary<string, string> SavedProgramPathPairs { get { return _savedProgramPathPairs;  } }


        public static string LocateProgramPath(
                string programName,
                IFileSystem fileSystem,
                IDrivesGetter drivesGetter,
                IOSPlatformChecker osPlatformChecker)
        {
            if(_savedProgramPathPairs.ContainsKey(programName) == true
                    && fileSystem.File.Exists(_savedProgramPathPairs[programName]) == true)
            {
                return _savedProgramPathPairs[programName];
            }

            var ending = string.Empty;
            if(osPlatformChecker.IsOSPlatform(OSPlatform.Windows))
            {
                ending = WindowsProgramEnding;
            }
            else if(osPlatformChecker.IsOSPlatform(OSPlatform.OSX))
            {
                ending = MacOSProgramEnding;
            }

            var searchPattern = "*" + programName + ending;
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

            if(files.Count == 0)
            {
                return null;
            }

            var programPath = files[0];
            _savedProgramPathPairs[programName] = programPath;

            return programPath;
        }
    }
}
