using System;
using System.Collections.Generic;
using System.IO;
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
        private const int CategoryIdIndexOffset = 3;
        private const int CategoryTypeIndexOffset = 4;
        private const float DataPointStringAppearanceThreshold = 0.25f;
        private const float DataPointStringCorrelationThreshold = 0.75f;
        private const int MinRequiredSnapshots = 100;
        private const int SnapshotKeepLookbackDays = 7;
        private const int SnapshotTypeIndexOffset = 2;
        private const float ThresholdStepIncreaseAmount = 0.05f;


        public static List<Hypothesis> Create(
                IFileSystem fileSystem,
                IClock clock,
                ICurrentDomainBaseDirectoryGetter currentDomainBaseDirectoryGetter)
        {
            var aggregateHypotheses = new List<Hypothesis>();
            var snapshotDirectoryPaths = GetSnapshotDirectoryPaths(fileSystem,
                    currentDomainBaseDirectoryGetter);

            foreach(string directoryPath in snapshotDirectoryPaths)
            {
                SnapshotDeleter.DeleteOutdatedFiles(directoryPath, SnapshotKeepLookbackDays, fileSystem,
                        clock);
                var snapshots = SnapshotsLoader.Load(directoryPath, fileSystem);

                if(snapshots.Count < MinRequiredSnapshots)
                {
                    continue;
                }

                var trueCounts = CountNumberOfTruesForEachDataPointString(snapshots);
                var filteredDataPointStrings = FilterDataPointStringsByTrueCounts(snapshots, trueCounts);
                var snapshotHashSets = ConvertSnapshotsToHashSets(snapshots);
                var dataPoints = CreateDataPoints(snapshotHashSets, filteredDataPointStrings);
                var clusteringResult = GetClusteringResult(dataPoints);
                var hypotheses = CreateHypotheses(directoryPath, snapshots, clusteringResult, trueCounts);
                hypotheses = FilterHypothesesByExistence(hypotheses, snapshotHashSets,
                        filteredDataPointStrings);

                aggregateHypotheses.AddRange(hypotheses);
            }

            return aggregateHypotheses;
        }

        private static HashSet<string> GetSnapshotDirectoryPaths(
                IFileSystem fileSystem,
                ICurrentDomainBaseDirectoryGetter currentDomainBaseDirectoryGetter)
        {
            var directoryPathElements = new string[]
            {
                currentDomainBaseDirectoryGetter.GetCurrentDomainBaseDirectory(),
                "Data",
                "Snapshots"
            };
            var topDirectoryPath = Path.Combine(directoryPathElements);
            string[] filePaths;

            try
            {
                filePaths = fileSystem.Directory.GetFiles(topDirectoryPath, "*",
                        SearchOption.AllDirectories);
            }
            catch
            {
                return new HashSet<string>();
            }

            var directoryPaths = new HashSet<string>();

            foreach(string filePath in filePaths)
            {
                var filePathElements = filePath.Split(@"/\".ToCharArray()).ToList();
                filePathElements.RemoveAt(filePathElements.Count - 1);
                directoryPaths.Add(Path.Combine(filePathElements.ToArray()));
            }

            return directoryPaths;
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
                string directoryPath,
                List<List<string>> snapshots,
                ClusteringResult<DataPoint> clusteringResult,
                Dictionary<string, int> trueCounts)
        {
            var hypotheses = new List<Hypothesis>();

            for(int i = 0; i < clusteringResult.Count; i++)
            {
                hypotheses.AddUniques(ConvertClusterSetToHypotheses(directoryPath, clusteringResult[i]));
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

        private static List<Hypothesis> ConvertClusterSetToHypotheses(
                string directoryPath,
                ClusterSet<DataPoint> clusterSet)
        {
            var hypotheses = new List<Hypothesis>();
            var directoryPathElements = directoryPath.Split(@"/\".ToCharArray());
            var categoryType = directoryPathElements[directoryPathElements.Length - CategoryTypeIndexOffset];
            var categoryId = directoryPathElements[directoryPathElements.Length - CategoryIdIndexOffset];
            var snapshotType = directoryPathElements[directoryPathElements.Length - SnapshotTypeIndexOffset];

            foreach(Cluster<DataPoint> cluster in clusterSet)
            {
                var dataPointStrings = new HashSet<string>();

                foreach(DataPoint dataPoint in cluster)
                {
                    dataPointStrings.Add(dataPoint.Name);
                }

                hypotheses.Add(new Hypothesis(categoryType, categoryId, snapshotType, dataPointStrings));
            }

            return hypotheses;
        }

        /// The hypotheses that get passed to this function may include combinations of DataPointStrings
        /// that never showed up in any of the snapshots being evaluated. In order to not be removed,
        /// there needs to be at least one snapshot that contains all of the DataPointStrings in the
        /// hypothesis, and none of the DataPointStrings that are in filteredDataPointStrings but not in
        /// the hypothesis.
        private static List<Hypothesis> FilterHypothesesByExistence(
                List<Hypothesis> hypotheses,
                List<HashSet<string>> snapshotHashSets,
                List<string> filteredDataPointStrings)
        {
            for(int i = hypotheses.Count - 1; i >= 0; i--)
            {
                var hypothesis = hypotheses[i];
                var excluded = new List<string>(filteredDataPointStrings);
                excluded.RemoveAll(s => hypothesis.DataPointStrings.Contains(s));

                for(int j = 0; j < snapshotHashSets.Count; j++)
                {
                    if(snapshotHashSets[j].All(s => hypothesis.DataPointStrings.Contains(s))
                            && !snapshotHashSets[j].Any(s => excluded.Contains(s)))
                    {
                        break;
                    }
                    else if(j == snapshotHashSets.Count - 1)
                    {
                        hypotheses.RemoveAt(i);
                    }
                }
            }

            return hypotheses;
        }


        private class JaccardDissimilarityMetric : IDissimilarityMetric<DataPoint>
        {
            public double Calculate(DataPoint instance1, DataPoint instance2)
            {
                var similarity = (double)DataAnalysisHelpers.CalculateJaccardSimilarity(instance1.Values,
                        instance2.Values);
                var dissimilarity = 1 - similarity;
                
                /// This is to act as a tie breaker in situations where there are two pairs of values that
                /// have the same dissimilarity score. In those cases, the incorrect pair may be selected
                /// to be grouped together. We assume that it's more likely for pairs that have more true
                /// values to be part of the correct pair. At the same time, this value is only meant as
                /// a tie breaker, so it's important that the change it makes to the base dissimilarity
                /// score is very small.
                var trueCountTotal = (double)(instance1.Values.Sum() + instance2.Values.Sum());
                dissimilarity -= (double)(trueCountTotal / (instance1.Values.Count * 2) / 100);

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
