using System.Collections.Generic;
using System.IO.Abstractions;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Types
{
    public static class HypothesesCreator
    {
        private const int SnapshotKeepLookbackDays = 7;


        public static List<Hypothesis> Create(
                IFileSystem fileSystem,
                IClock clock)
        {
            var hypotheses = new List<Hypothesis>();

            SnapshotDeleter.DeleteOutdatedFiles(@"C:\", SnapshotKeepLookbackDays, fileSystem, clock);
            hypotheses.Add(new Hypothesis());

            return hypotheses;
        }
    }
}
