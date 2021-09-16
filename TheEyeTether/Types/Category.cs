using System.Collections.Generic;

namespace TheEyeTether.Types
{
    public record Category
    {
        public List<TimestampRange> ActiveTimePeriods;
        public string Identifier;


        public Category(string identifier, List<TimestampRange> activeTimePeriods) =>
                (ActiveTimePeriods, Identifier) = (activeTimePeriods, identifier);
    }
}
