using System.Collections.Generic;

namespace TheEyeTether.Types
{
    public static class SnapshotsLoader
    {
        public static List<Snapshot> Load(
                string directoryPath,
                int lookbackDays)
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

            return new List<Snapshot>();
        }
    }
}
