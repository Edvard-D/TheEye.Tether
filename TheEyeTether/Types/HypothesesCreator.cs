using System.Collections.Generic;
using System.IO.Abstractions;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Types
{
    public static class HypothesesCreator
    {
        private const float DataPointStringAppearanceThreshold = 0.25f;
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
            var snapshotsCount = (float)snapshots.Count;

            if(snapshotsCount < MinRequiredSnapshots)
            {
                return new List<Hypothesis>();
            }

            var hypothesis = new Hypothesis();
            var appearanceCounts = CountNumberOfAppearancesForEachDataPointString(snapshots);

            foreach(KeyValuePair<string, int> keyValuePair in appearanceCounts)
            {
                if(keyValuePair.Value / snapshotsCount < DataPointStringAppearanceThreshold)
                {
                    continue;
                }

                hypothesis.DataPointStrings.Add(keyValuePair.Key);
            }

            hypotheses.Add(hypothesis);

            return hypotheses;
        }

        private static Dictionary<string, int> CountNumberOfAppearancesForEachDataPointString(
                List<List<string>> snapshots)
        {
            var appearanceCounts = new Dictionary<string, int>();

            foreach(List<string> snapshot in snapshots)
            {
                foreach(string dataPointString in snapshot)
                {
                    if(!appearanceCounts.ContainsKey(dataPointString))
                    {
                        appearanceCounts[dataPointString] = 1;
                        continue;
                    }

                    appearanceCounts[dataPointString] += 1;
                }
            }

            return appearanceCounts;
        }
    }
}
