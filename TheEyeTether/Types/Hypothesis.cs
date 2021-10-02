using System.Collections.Generic;

namespace TheEyeTether.Types
{
    public record Hypothesis
    {
        public string CategoryType;
        public HashSet<string> DataPointStrings;


        public Hypothesis(string categoryType)
        {
            CategoryType = categoryType;
            DataPointStrings = new HashSet<string>();
        }
        public Hypothesis(
                string categoryType,
                HashSet<string> dataPointStrings)
        {
            CategoryType = categoryType;
            DataPointStrings = dataPointStrings;
        }
        public Hypothesis(HashSet<string> dataPointStrings)
        {
            CategoryType = string.Empty;
            DataPointStrings = dataPointStrings;
        }
    }
}
