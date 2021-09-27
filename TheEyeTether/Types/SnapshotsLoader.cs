using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text.Json;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Types
{
    public static class SnapshotsLoader
    {
        private const string FilePathDateTimeFormat = "yyyy_MM_dd__HH_mm_ss";


        public static List<List<string>> Load(
                string directoryPath,
                int lookbackDays,
                IFileSystem fileSystem,
                IClock clock)
        {
            if(directoryPath == null)
            {
                throw new System.InvalidOperationException(string.Format("Argument {0} cannot be null.",
                        nameof(directoryPath)));
            }

            if(lookbackDays <= 0)
            {
                throw new System.InvalidOperationException(string.Format("Argument {0} must be positive.",
                        nameof(lookbackDays)));
            }

            var snapshots = new List<List<string>>();
            foreach(string filePath in fileSystem.Directory.GetFiles(directoryPath))
            {
                System.DateTime fileDateTime;
                try
                {
                    var fileName = fileSystem.Path.GetFileName(filePath);
                    fileName = fileName.Split(".")[0];
                    fileDateTime = System.DateTime.ParseExact(fileName, FilePathDateTimeFormat, null);
                }
                catch
                {
                    continue;
                }
                
                var elapsedTime = clock.Now - fileDateTime;
                if(elapsedTime.Days > lookbackDays)
                {
                    continue;
                }

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
