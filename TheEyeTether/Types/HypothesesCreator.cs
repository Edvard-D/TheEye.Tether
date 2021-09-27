using System.Collections.Generic;
using System.IO.Abstractions;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Types
{
    public static class HypothesesCreator
    {
        private const int MinRequiredSnapshots = 100;
        private const int SnapshotKeepLookbackDays = 7;


        public static List<Hypothesis> Create(
                IFileSystem fileSystem,
                IClock clock)
        {
            var hypotheses = new List<Hypothesis>();

            var directoryPath = @"C:\";
            SnapshotDeleter.DeleteOutdatedFiles(directoryPath, SnapshotKeepLookbackDays, fileSystem, clock);
            var snapshots = SnapshotsLoader.Load(directoryPath, fileSystem);

            if(snapshots.Count >= MinRequiredSnapshots)
            {
                hypotheses.Add(new Hypothesis());
            }

            return hypotheses;
        }
    }
}
