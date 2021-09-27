using System.IO.Abstractions;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Types
{
    public static class SnapshotDeleter
    {
        public static void DeleteOutdatedFiles(
                string directoryPath,
                int keepLookbackDays,
                IFileSystem fileSystem,
                IClock clock)
        {
            if(directoryPath == null)
            {
                throw new System.InvalidOperationException(string.Format("Argument {0} cannot be null.",
                        nameof(directoryPath)));
            }

            if(keepLookbackDays <= 0)
            {
                throw new System.InvalidOperationException(string.Format("Argument {0} must be positive.",
                        nameof(keepLookbackDays)));
            }

            foreach(string filePath in fileSystem.Directory.GetFiles(directoryPath))
            {
                var creationDateTime = fileSystem.File.GetCreationTimeUtc(filePath);
                var keepThresholdDateTime = clock.Now.ToUniversalTime().AddDays(-keepLookbackDays);

                if(creationDateTime < keepThresholdDateTime)
                {
                    fileSystem.File.Delete(filePath);
                }
            }
        }
    }
}
