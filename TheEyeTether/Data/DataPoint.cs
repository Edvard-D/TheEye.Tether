namespace TheEye.Tether.Data
{
	public record DataPoint
	{
		public string SubTypeName;
		public TimestampRange TimestampRange;
		public string TypeName;


		public DataPoint(string typeName, string subTypeName, TimestampRange timestampRange) =>
				(SubTypeName, TimestampRange, TypeName) = (subTypeName, timestampRange, typeName);
	}
}
