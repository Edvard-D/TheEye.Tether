using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using TheEyeTether.Helpers;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Types
{
    public static class HypothesesCreator
    {
        private const float DataPointStringAppearanceThreshold = 0.25f;
        private const float DataPointStringCorrelationThreshold = 0.75f;
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

            if(snapshots.Count < MinRequiredSnapshots)
            {
                return new List<Hypothesis>();
            }

            var filteredDataPointStrings = FilterDataPointStringsByAppearanceCounts(snapshots);
            var appearanceValues = AggregateAppearanceValues(snapshots, filteredDataPointStrings);
            var correlations = CalculateDataPointStringPairCorrelations(filteredDataPointStrings,
                    appearanceValues);
            hypotheses.AddRange(CreateHypothesesFromCorrelations(correlations));

            return hypotheses;
        }

        private static List<string> FilterDataPointStringsByAppearanceCounts(List<List<string>> snapshots)
        {
            var filteredDataPointStrings = new List<string>();
            var appearanceCounts = CountNumberOfAppearancesForEachDataPointString(snapshots);
            var snapshotsCount = (float)snapshots.Count;

            foreach(KeyValuePair<string, int> keyValuePair in appearanceCounts)
            {
                if(keyValuePair.Value / snapshotsCount < DataPointStringAppearanceThreshold)
                {
                    continue;
                }

                filteredDataPointStrings.Add(keyValuePair.Key);
            }

            return filteredDataPointStrings;
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

        private static Dictionary<string, List<int>> AggregateAppearanceValues(
                List<List<string>> snapshots,
                List<string> filteredDataPointStrings)
        {
            var snapshotHashSets = ConvertSnapshotsToHashSets(snapshots);
            var appearanceValues = new Dictionary<string, List<int>>();

            foreach(string dataPointString in filteredDataPointStrings)
            {
                var dataPointStringAppearanceValues = new List<int>();

                foreach(HashSet<string> snapshotHashSet in snapshotHashSets)
                {
                    var containsKey = snapshotHashSet.Contains(dataPointString);
                    dataPointStringAppearanceValues.Add(System.Convert.ToInt32(containsKey));
                }

                appearanceValues[dataPointString] = dataPointStringAppearanceValues;
            }

            return appearanceValues;
        }
        
        private static List<HashSet<string>> ConvertSnapshotsToHashSets(List<List<string>> snapshots)
        {
            var snapshotHashSets = new List<HashSet<string>>();

            foreach(List<string> snapshot in snapshots)
            {
                var snapshotHashSet = new HashSet<string>();

                foreach(string dataPointString in snapshot)
                {
                    snapshotHashSet.Add(dataPointString);
                }

                snapshotHashSets.Add(snapshotHashSet);
            }

            return snapshotHashSets;
        }

        private static Dictionary<string, Dictionary<string, float>> CalculateDataPointStringPairCorrelations(
                List<string> filteredDataPointStrings,
                Dictionary<string, List<int>> appearanceValues)
        {
            var correlations = new Dictionary<string, Dictionary<string, float>>();

            for(int i = 0; i < filteredDataPointStrings.Count; i++)
            {
                var iDataPointString = filteredDataPointStrings[i];
                var dataPointStringCorrelations = new Dictionary<string, float>();

                for(int j = 0; j < filteredDataPointStrings.Count; j++)
                {
                    if(i == j)
                    {
                        continue;
                    }

                    var jDataPointString = filteredDataPointStrings[j];

                    /// We can reuse value that was already calculated.
                    if(correlations.ContainsKey(jDataPointString))
                    {
                        dataPointStringCorrelations[jDataPointString] =
                                correlations[jDataPointString][iDataPointString];
                        
                        continue;
                    }

                    dataPointStringCorrelations[jDataPointString] =
                            DataAnalysisHelpers.CalculateJaccardSimilarity(
                                    appearanceValues[iDataPointString], appearanceValues[jDataPointString]);
                }

                correlations[iDataPointString] = dataPointStringCorrelations;
            }

            return correlations;
        }

        private static List<Hypothesis> CreateHypothesesFromCorrelations(
                Dictionary<string, Dictionary<string, float>> correlations)
        {
            var hypotheses = new List<Hypothesis>();

            foreach(KeyValuePair<string, Dictionary<string, float>> correlationKeyValuePair in correlations)
            {
                var filteredCorrelations = correlationKeyValuePair.Value
                        .Where(kvp => kvp.Value >= DataPointStringCorrelationThreshold)
                        .ToList();
                
                if(filteredCorrelations.Count == 0)
                {
                    hypotheses.Add(new Hypothesis(new HashSet<string>() { correlationKeyValuePair.Key }));
                    continue;
                }
                
                var validCorrelations = new HashSet<string>();
                
                /// We want to find the DataPointStrings that correlate with all the other filtered
                /// DataPointStrings. We can filter out the ones that don't correlate with all of the
                /// other DataPointStrings that correlate with the correlationKeyValuePair being checked.
                /// This is because it will be added as its own Hypothesis if it hasn't already been.
                foreach(KeyValuePair<string, float> filteredCorrelationKeyValuePair in filteredCorrelations)
                {
                    var filteredCorrelationKey = filteredCorrelationKeyValuePair.Key;
                    var doesCorrelateWithAllOthers = filteredCorrelations
                            .All(kvp => filteredCorrelationKey == kvp.Key ||
                                    correlations[filteredCorrelationKey][kvp.Key] >=
                                            DataPointStringCorrelationThreshold);

                    if(doesCorrelateWithAllOthers == true)
                    {
                        validCorrelations.Add(filteredCorrelationKey);
                    }
                }
                
                validCorrelations.Add(correlationKeyValuePair.Key);
                
                if(!DoesHypothesisExist(validCorrelations, hypotheses))
                {
                    hypotheses.Add(new Hypothesis(validCorrelations));
                }
            }

            return hypotheses;
        }

        private static bool DoesHypothesisExist(
                HashSet<string> dataPointStrings,
                List<Hypothesis> hypotheses)
        {
            return hypotheses.Any(h => h.DataPointStrings.SetEquals(dataPointStrings));
        }
    }
}
