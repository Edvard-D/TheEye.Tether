using System.Collections.Generic;

namespace TheEyeTether.Types
{
    public record Category
    {
        public List<TimestampRange> ActiveTimePeriods;
        public string Identifier;
        public CategorySetting Setting;


        public Category(string identifier, CategorySetting setting, List<TimestampRange> activeTimePeriods) =>
                (ActiveTimePeriods, Identifier, Setting) = (activeTimePeriods, identifier, setting);
    }
}
