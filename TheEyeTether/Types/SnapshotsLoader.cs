using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text.Json;

namespace TheEyeTether.Types
{
    public static class SnapshotsLoader
    {
        public static List<List<string>> Load(
                string directoryPath,
                int lookbackDays,
                IFileSystem fileSystem)
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
                var fileText = fileSystem.File.ReadAllText(filePath);

                try
                {
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
