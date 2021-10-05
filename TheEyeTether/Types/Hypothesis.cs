using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TheEyeTether.Types
{
    public record Hypothesis
    {
        public string CategoryId;
        public string CategoryType;
        public HashSet<string> DataPointStrings;
        public DateTime SentDateTime;
        public string SnapshotId;
        public string SnapshotType;
        public bool WasSent;


        public Hypothesis(
                string categoryType,
                string categoryId,
                string snapshotType,
                string snapshotId)
        {
            CategoryId = categoryId;
            CategoryType = categoryType;
            DataPointStrings = new HashSet<string>();
            SentDateTime = DateTime.UnixEpoch;
            SnapshotId = snapshotId;
            SnapshotType = snapshotType;
            WasSent = false;
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
            SentDateTime = DateTime.UnixEpoch;
            SnapshotId = snapshotId;
            SnapshotType = snapshotType;
            WasSent = false;
        }
        [JsonConstructor]
        public Hypothesis(
                string categoryType,
                string categoryId,
                string snapshotType,
                string snapshotId,
                HashSet<string> dataPointStrings,
                DateTime sentDateTime,
                bool wasSent)
        {
            CategoryId = categoryId;
            CategoryType = categoryType;
            DataPointStrings = dataPointStrings;
            SentDateTime = sentDateTime;
            SnapshotId = snapshotId;
            SnapshotType = snapshotType;
            WasSent = wasSent;
        }
        public Hypothesis(HashSet<string> dataPointStrings)
        {
            CategoryId = string.Empty;
            CategoryType = string.Empty;
            DataPointStrings = dataPointStrings;
            SentDateTime = DateTime.UnixEpoch;
            SnapshotId = string.Empty;
            SnapshotType = string.Empty;
            WasSent = false;
        }
    }
}
