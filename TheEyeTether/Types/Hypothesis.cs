using System.Collections.Generic;

namespace TheEyeTether.Types
{
    public record Hypothesis
    {
        public HashSet<string> DataPointStrings;


        public Hypothesis()
        {
            DataPointStrings = new HashSet<string>();
        }
        public Hypothesis(HashSet<string> dataPointStrings)
        {
            DataPointStrings = dataPointStrings;
        }
    }
}
