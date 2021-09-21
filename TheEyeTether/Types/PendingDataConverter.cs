using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text.Json;
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
        private const string OutputFilePathDateTimeFormat = "yyyy_MM_dd__HH_mm_ss";
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
                ICurrentDomainBaseDirectoryGetter currentDomainBaseDirectoryGetter,
                IClock clock)
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

                foreach(KeyValuePair<Category, Dictionary<SnapshotSetting, List<Snapshot>>> categoryKeyValuePair in snapshots)
                {
                    foreach(KeyValuePair<SnapshotSetting, List<Snapshot>> snapshotSettingKeyValuePair in categoryKeyValuePair.Value)
                    {
                        var groupedSnapshots = GroupSnapshotsBySubTypeName(snapshotSettingKeyValuePair.Value);
                        
                        foreach(KeyValuePair<string, List<Snapshot>> groupedSnapshotKeyValuePair in groupedSnapshots)
                        {
                            var outputFilePath = CreateOutputFilePath(filePath, categoryKeyValuePair.Key,
                                    snapshotSettingKeyValuePair.Key, groupedSnapshotKeyValuePair.Key,
                                    fileSystem, currentDomainBaseDirectoryGetter, clock);
                            
                            var outputData = new List<List<string>>();
                            foreach(Snapshot snapshot in groupedSnapshotKeyValuePair.Value)
                            {
                                outputData.Add(snapshot.DataPointsIds);
                            }

                            var outputJson = JsonSerializer.Serialize(outputData);
                            fileSystem.File.WriteAllText(outputFilePath, outputJson);
                        }
                    }
                }
            }
        }

        private static Dictionary<string, List<Snapshot>> GroupSnapshotsBySubTypeName(
                List<Snapshot> snapshots)
        {
            var groupedSnapshots = new Dictionary<string, List<Snapshot>>();

            foreach(Snapshot snapshot in snapshots)
            {
                var snapshotSubTypeName = snapshot.DataPoint.SubTypeName;
                if(snapshotSubTypeName == null)
                {
                    snapshotSubTypeName = "default";
                }

                if(!groupedSnapshots.ContainsKey(snapshotSubTypeName))
                {
                    groupedSnapshots[snapshotSubTypeName] = new List<Snapshot>();
                }

                groupedSnapshots[snapshotSubTypeName].Add(snapshot);
            }

            return groupedSnapshots;
        }

        private static string CreateOutputFilePath(
                string inputFilePath,
                Category category,
                SnapshotSetting snapshotSetting,
                string snapshotSubTypeName,
                IFileSystem fileSystem,
                ICurrentDomainBaseDirectoryGetter currentDomainBaseDirectoryGetter,
                IClock clock)
        {
            var inputFilePathElements = inputFilePath.Split(@"/\".ToCharArray());
            var now = clock.Now.ToString(OutputFilePathDateTimeFormat);
            
            var outputFilePathParts = new string[]
            {
                currentDomainBaseDirectoryGetter.GetCurrentDomainBaseDirectory(),
                "Data",
                "Snapshots",
                category.Setting.Name,
                category.Identifier,
                snapshotSetting.Name,
                snapshotSubTypeName
            };

            var outputFilePath = Path.Combine(outputFilePathParts);
            fileSystem.Directory.CreateDirectory(outputFilePath);
            outputFilePath = Path.Combine(outputFilePath, now + ".json");
            
            return outputFilePath;
        }
    }
}
