using System.IO.Abstractions;

namespace TheEyeTether.Types
{
    public static class SnapshotDeleter
    {
        public static void DeleteOutdatedFiles(
                string directoryPath,
                int keepLookbackDays,
                IFileSystem fileSystem)
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
        }
    }
}
