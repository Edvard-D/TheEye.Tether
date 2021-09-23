using System.Collections.Generic;

namespace TheEyeTether.Types
{
    public static class SnapshotsLoader
    {
        public static List<Snapshot> Load(string directoryPath)
        {
            if(directoryPath == null)
            {
                throw new System.InvalidOperationException(string.Format("Argument {0} cannot be null.",
                        nameof(directoryPath)));
            }

            return new List<Snapshot>();
        }
    }
}
