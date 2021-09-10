using System.IO.Abstractions;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Types
{
    public static class PendingDataConverter
    {
        private const string FileName = "TheEyeRecorder.lua";
        private const string ProgramName = "Wow";
        private const string RequiredDirectories = @"WorldOfWarcraft\_retail_\";        


        public static void Execute(
                IFileSystem fileSystem,
                IDrivesGetter drivesGetter,
                IOSPlatformChecker osPlatformChecker)
        {
            var programPath = ProgramPathLocater.LocateProgramPath(ProgramName, RequiredDirectories,
                    fileSystem, drivesGetter, osPlatformChecker);
            var searchDirectoryPath = programPath.Replace(ProgramName + ".exe", string.Empty);
            var filePaths = FilePathAggregator.AggregateFilePaths(FileName, searchDirectoryPath, fileSystem);

            foreach(string filePath in filePaths)
            {
                var newFilePath = @"C:\test.txt";
                fileSystem.File.Create(newFilePath);
            }
        }
    }
}
