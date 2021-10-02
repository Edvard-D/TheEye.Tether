using System.Collections.Generic;

namespace TheEyeTether.Types
{
    public record Hypothesis
    {
        public string CategoryId;
        public string CategoryType;
        public HashSet<string> DataPointStrings;
        public string SnapshotId;
        public string SnapshotType;


        public Hypothesis(
                string categoryType,
                string categoryId,
                string snapshotType,
                string snapshotId)
        {
            CategoryId = categoryId;
            CategoryType = categoryType;
            DataPointStrings = new HashSet<string>();
            SnapshotId = snapshotId;
            SnapshotType = snapshotType;
        }
        public Hypothesis(
                string categoryType,
                string categoryId,
                string snapshotType,
                string snapshotId,
                HashSet<string> dataPointStrings)
        {
            CategoryId = categoryId;
            CategoryType = categoryType;
            DataPointStrings = dataPointStrings;
            SnapshotId = snapshotId;
            SnapshotType = snapshotType;
        }
        public Hypothesis(HashSet<string> dataPointStrings)
        {
            CategoryId = string.Empty;
            CategoryType = string.Empty;
            DataPointStrings = dataPointStrings;
            SnapshotId = string.Empty;
            SnapshotType = string.Empty;
        }
    }
}
