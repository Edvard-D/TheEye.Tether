using System.Collections.Generic;
using System.Linq;
using TheEyeTether.Types;

namespace TheEyeTether.Extensions
{
    public static class ListHypothesisExtensions
    {
        public static void AddUnique(
                this List<Hypothesis> list,
                Hypothesis hypothesis)
        {
            if(list.Any(h => h.DataPointStrings.SetEquals(hypothesis.DataPointStrings)))
            {
                return;
            }

            list.Add(hypothesis);
        }

        public static void AddUniques(
                this List<Hypothesis> list,
                List<Hypothesis> hypotheses)
        {
            list.AddRange(hypotheses);
        }
    }
}
