using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using TheEyeTether.Helpers;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Types
{
    public static class PendingDataConverter
    {
        private const int AccountNameElementOffset = 5;
        private const int CharacterNameElementOffset = 3;
        private const string FileName = "TheEyeRecorder.lua";
        private const string LuaTableName = "TheEyeRecordedData";
        private const string ProgramName = "Wow";
        private const string RequiredDirectories = @"WorldOfWarcraft\_retail_\";
        private const int ServerNameElementOffset = 4;


        public static void Convert(
                Dictionary<string, CategorySetting> categorySettings,
                Dictionary<string, DataPointSetting> dataPointSettings,
                IFileSystem fileSystem,
                ILua lua,
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
                lua.DoFile(filePath);
                var luaTable = lua.ConvertTableHierarchyToDict(lua.GetTable(LuaTableName));
                var snapshots = SnapshotsCreator.Create(luaTable, categorySettings, dataPointSettings);

                foreach(KeyValuePair<Category, Dictionary<SnapshotSetting, List<Snapshot>>> keyValuePair in snapshots)
                {
                    var outputFilePath = CreateOutputFilePath(filePath, keyValuePair.Key, fileSystem,
                            currentDomainBaseDirectoryGetter);
                    fileSystem.File.Create(outputFilePath);
                }
            }
        }

        private static string CreateOutputFilePath(
                string inputFilePath,
                Category category,
                IFileSystem fileSystem,
                ICurrentDomainBaseDirectoryGetter currentDomainBaseDirectoryGetter)
        {
            var inputFilePathElements = inputFilePath.Split(@"/\".ToCharArray());

            var outputFilePathParts = new string[]
            {
                currentDomainBaseDirectoryGetter.GetCurrentDomainBaseDirectory(),
                "Data",
                "Snapshots",
                category.Setting.Name,
                "test.txt"
            };

            var outputFilePath = Path.Combine(outputFilePathParts);
            fileSystem.Directory.CreateDirectory(outputFilePath);
            
            return outputFilePath;
        }
    }
}
