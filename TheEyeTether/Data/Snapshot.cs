using System.Collections.Generic;

namespace TheEyeTether.Data
{
	public record Snapshot
	{
		public DataPoint DataPoint;
		public List<string> DataPointsIds;


		public Snapshot(DataPoint dataPoint)
		{
			DataPoint = dataPoint;
			DataPointsIds = new List<string>();
		}
	}
}
