using System.Collections.Generic;
using TheEyeTether.Types;

namespace TheEyeTether.Extensions
{
    public static class ListHypothesisExtensions
    {
        public static void AddUnique(
                this List<Hypothesis> list,
                Hypothesis hypothesis)
        {
            list.Add(hypothesis);
        }
    }
}
