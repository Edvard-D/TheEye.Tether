using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Aglomera;
using Aglomera.Evaluation.Internal;
using Aglomera.Linkage;
using TheEyeTether.Extensions;
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
        private const float ThresholdStepIncreaseAmount = 0.05f;


        public static List<Hypothesis> Create(
                IFileSystem fileSystem,
                IClock clock)
        {
            var directoryPath = @"C:\";
            SnapshotDeleter.DeleteOutdatedFiles(directoryPath, SnapshotKeepLookbackDays, fileSystem, clock);
            var snapshots = SnapshotsLoader.Load(directoryPath, fileSystem);

            if(snapshots.Count < MinRequiredSnapshots)
            {
                return new List<Hypothesis>();
            }

            var trueCounts = CountNumberOfTruesForEachDataPointString(snapshots);
            var filteredDataPointStrings = FilterDataPointStringsByTrueCounts(snapshots, trueCounts);
            var snapshotHashSets = ConvertSnapshotsToHashSets(snapshots);
            var dataPoints = CreateDataPoints(snapshotHashSets, filteredDataPointStrings);
            var clusteringResult = GetClusteringResult(dataPoints);
            var hypotheses = CreateHypotheses(snapshots, clusteringResult, trueCounts);

            return hypotheses;
        }

        private static Dictionary<string, int> CountNumberOfTruesForEachDataPointString(
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

        private static List<string> FilterDataPointStringsByTrueCounts(
                List<List<string>> snapshots,
                Dictionary<string, int> trueCounts)
        {
            var filteredDataPointStrings = new List<string>();
            var snapshotsCount = (float)snapshots.Count;

            foreach(KeyValuePair<string, int> keyValuePair in trueCounts)
            {
                if(keyValuePair.Value / snapshotsCount < DataPointStringAppearanceThreshold)
                {
                    continue;
                }

                filteredDataPointStrings.Add(keyValuePair.Key);
            }

            return filteredDataPointStrings;
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

        private static HashSet<DataPoint> CreateDataPoints(
                List<HashSet<string>> snapshotHashSets,
                List<string> filteredDataPointStrings)
        {
            var dataPoints = new HashSet<DataPoint>();

            foreach(string dataPointString in filteredDataPointStrings)
            {
                var values = new List<int>();

                foreach(HashSet<string> snapshotHashSet in snapshotHashSets)
                {
                    var containsKey = snapshotHashSet.Contains(dataPointString);
                    values.Add(System.Convert.ToInt32(containsKey));
                }

                dataPoints.Add(new DataPoint(dataPointString, values));
            }

            return dataPoints;
        }

        private static ClusteringResult<DataPoint> GetClusteringResult(HashSet<DataPoint> dataPoints)
        {
            var dissimilarityMetric = new JaccardDissimilarityMetric();
            var linkage = new AverageLinkage<DataPoint>(dissimilarityMetric);
            var algorithm = new AgglomerativeClusteringAlgorithm<DataPoint>(linkage);
            var clusteringResult = algorithm.GetClustering(dataPoints);

            return clusteringResult;
        }

        private static List<Hypothesis> CreateHypotheses(
                List<List<string>> snapshots,
                ClusteringResult<DataPoint> clusteringResult,
                Dictionary<string, int> trueCounts)
        {
            var hypotheses = new List<Hypothesis>();

            for(int i = 0; i < clusteringResult.Count; i++)
            {
                hypotheses.AddUniques(ConvertClusterSetToHypotheses(clusteringResult[i]));
            }

            return hypotheses;
        }

        private static ClusterSet<DataPoint> GetBestClusterSet(
                ClusteringResult<DataPoint> clusteringResult)
        {
            var dissimilarityMetric = new JaccardDissimilarityMetric();
            var criterion = new SilhouetteCoefficient<DataPoint>(dissimilarityMetric);
            var bestClusterSet = clusteringResult[0];
            var bestScore = criterion.Evaluate(clusteringResult[0]);

            for(int i = 1; i < clusteringResult.Count; i++)
            {
                var clusterSet = clusteringResult[i];
                var score = criterion.Evaluate(clusterSet);

                if(score > bestScore)
                {
                    bestScore = score;
                    bestClusterSet = clusterSet;
                }
            }

            return bestClusterSet;
        }

        private static List<Hypothesis> ConvertClusterSetToHypotheses(ClusterSet<DataPoint> clusterSet)
        {
            var hypotheses = new List<Hypothesis>();

            foreach(Cluster<DataPoint> cluster in clusterSet)
            {
                var dataPointStrings = new HashSet<string>();

                foreach(DataPoint dataPoint in cluster)
                {
                    dataPointStrings.Add(dataPoint.Name);
                }

                hypotheses.Add(new Hypothesis(dataPointStrings));
            }

            return hypotheses;
        }


        private class JaccardDissimilarityMetric : IDissimilarityMetric<DataPoint>
        {
            public double Calculate(DataPoint instance1, DataPoint instance2)
            {
                var similarity = DataAnalysisHelpers.CalculateJaccardSimilarity(instance1.Values,
                        instance2.Values);
                var dissimilarity = 1 - similarity;

                return dissimilarity;
            }
        }


        private class DataPoint : IComparable<DataPoint>
        {
            public string Name;
            public List<int> Values;


            public DataPoint(string name, List<int> values)
            {
                Name = name;
                Values = values;
            }


            public int CompareTo(DataPoint other)
            {
                return this.Name.CompareTo(other.Name);
            }
        }
    }
}
