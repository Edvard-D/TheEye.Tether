using System.Collections.Generic;

namespace TheEyeTether.Types
{
    public record Hypothesis
    {
        public string CategoryId;
        public string CategoryType;
        public HashSet<string> DataPointStrings;
        public string SnapshotType;


        public Hypothesis(
                string categoryType,
                string categoryId,
                string snapshotType)
        {
            CategoryId = categoryId;
            CategoryType = categoryType;
            DataPointStrings = new HashSet<string>();
            SnapshotType = snapshotType;
        }
        public Hypothesis(
                string categoryType,
                string categoryId,
                string snapshotType,
                HashSet<string> dataPointStrings)
        {
            CategoryId = categoryId;
            CategoryType = categoryType;
            DataPointStrings = dataPointStrings;
            SnapshotType = snapshotType;
        }
        public Hypothesis(HashSet<string> dataPointStrings)
        {
            CategoryId = string.Empty;
            CategoryType = string.Empty;
            DataPointStrings = dataPointStrings;
            SnapshotType = string.Empty;
        }
    }
}
