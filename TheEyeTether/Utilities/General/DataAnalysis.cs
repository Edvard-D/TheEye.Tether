using System.Collections.Generic;

namespace TheEyeTether.Utilities.General
{
    public static class DataAnalysisUtilities
    {
        public static float CalculateJaccardSimilarity(
                List<int> listA,
                List<int> listB)
        {
            var commonValueCount = 0;
            for(int i = 0; i < listA.Count; i++)
            {
                if(i >= listB.Count)
                {
                    break;
                }

                if(listA[i] == listB[i])
                {
                    commonValueCount++;
                }
            }

            var combinedCount = listA.Count + listB.Count;

            return 2 * ((float)commonValueCount / (float)combinedCount);
        }
    }
}
