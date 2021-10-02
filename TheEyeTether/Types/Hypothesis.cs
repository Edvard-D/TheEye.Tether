using System.Collections.Generic;

namespace TheEyeTether.Types
{
    public record Hypothesis
    {
        public string CategoryId;
        public string CategoryType;
        public HashSet<string> DataPointStrings;


        public Hypothesis(
                string categoryType,
                string categoryId)
        {
            CategoryId = categoryId;
            CategoryType = categoryType;
            DataPointStrings = new HashSet<string>();
        }
        public Hypothesis(
                string categoryType,
                string categoryId,
                HashSet<string> dataPointStrings)
        {
            CategoryId = categoryId;
            CategoryType = categoryType;
            DataPointStrings = dataPointStrings;
        }
        public Hypothesis(HashSet<string> dataPointStrings)
        {
            CategoryId = string.Empty;
            CategoryType = string.Empty;
            DataPointStrings = dataPointStrings;
        }
    }
}
