using System.IO;
using System.IO.Abstractions;
using TheEyeTether.Helpers;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Types
{
    public static class PendingDataConverter
    {
        private const int AccountNameElementOffset = 5;
        private const string FileName = "TheEyeRecorder.lua";
        private const string ProgramName = "Wow";
        private const string RequiredDirectories = @"WorldOfWarcraft\_retail_\";        


        public static void Execute(
                IFileSystem fileSystem,
                IDrivesGetter drivesGetter,
                IOSPlatformChecker osPlatformChecker,
                ICurrentDomainBaseDirectoryGetter currentDomainBaseDirectoryGetter)
        {
            var programPath = ProgramPathLocater.LocateProgramPath(ProgramName, RequiredDirectories,
                    fileSystem, drivesGetter, osPlatformChecker);

            if(programPath == null)
            {
                return;
            }
            
            var programEnding = OSPlatformHelpers.GetProgramEnding(osPlatformChecker);
            var searchDirectoryPath = programPath.Replace(ProgramName + programEnding, string.Empty);
            var filePaths = FilePathAggregator.AggregateFilePaths(FileName, searchDirectoryPath, fileSystem);

            foreach(string filePath in filePaths)
            {
                var outputFilePath = CreateOutputFilePath(filePath, fileSystem,
                        currentDomainBaseDirectoryGetter);
                fileSystem.File.Create(outputFilePath);
            }
        }

        private static string CreateOutputFilePath(
                string inputFilePath,
                IFileSystem fileSystem,
                ICurrentDomainBaseDirectoryGetter currentDomainBaseDirectoryGetter)
        {
            var inputFilePathElements = inputFilePath.Split(@"/\".ToCharArray());

            var outputFilePathParts = new string[]
            {
                currentDomainBaseDirectoryGetter.GetCurrentDomainBaseDirectory(),
                "Data",
                "Snapshots",
                inputFilePathElements[inputFilePathElements.Length - AccountNameElementOffset],
                "test.txt"
            };

            var outputFilePath = Path.Combine(outputFilePathParts);
            fileSystem.Directory.CreateDirectory(outputFilePath);
            
            return outputFilePath;
        }
    }
}
