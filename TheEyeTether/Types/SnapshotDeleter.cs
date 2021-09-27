namespace TheEyeTether.Types
{
    public static class SnapshotDeleter
    {
        public static void DeleteOutdatedFiles(
                string directoryPath)
        {
            if(directoryPath == null)
            {
                throw new System.InvalidOperationException(string.Format("Argument {0} cannot be null.",
                        nameof(directoryPath)));
            }
        }
    }
}
