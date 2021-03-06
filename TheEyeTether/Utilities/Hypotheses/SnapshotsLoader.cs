using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text.Json;

namespace TheEyeTether.Utilities.Hypotheses
{
    public static class SnapshotsLoader
    {
        private const string FilePathDateTimeFormat = "yyyy_MM_dd__HH_mm_ss";


        public static List<List<string>> Load(
                string directoryPath,
                IFileSystem fileSystem)
        {
            var snapshots = new List<List<string>>();
            foreach(string filePath in fileSystem.Directory.GetFiles(directoryPath))
            {
                try
                {
                    var fileText = fileSystem.File.ReadAllText(filePath);
                    var jsonData = JsonSerializer.Deserialize<List<List<string>>>(fileText);
                    snapshots.AddRange(jsonData);
                }
                catch
                {
                    continue;
                }
            }

            return snapshots;
        }
    }
}
