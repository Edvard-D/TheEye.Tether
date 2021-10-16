using System.Collections.Generic;
using System.Linq;
using TheEye.Tether.Data;

namespace TheEye.Tether.Extensions
{
	public static class ListHypothesisExtensions
	{
		public static void AddUnique(
				this List<Hypothesis> list,
				Hypothesis hypothesis)
		{
			if(list.Any(h => h.CategoryType == hypothesis.CategoryType
					&& h.CategoryId == hypothesis.CategoryId
					&& h.SnapshotType == hypothesis.SnapshotType
					&& h.SnapshotId == hypothesis.SnapshotId
					&& h.DataPointStrings.SetEquals(hypothesis.DataPointStrings)))
			{
				return;
			}

			list.Add(hypothesis);
		}

		public static void AddUniques(
				this List<Hypothesis> list,
				List<Hypothesis> hypotheses)
		{
			for(int i = 0; i < hypotheses.Count; i++)
			{
				list.AddUnique(hypotheses[i]);
			}
		}
	}
}
