namespace TheEye.Tether.Data
{
	public record SnapshotSetting
	{
		public string[] DataPointTypeNames;
		public string Name;


		public SnapshotSetting(string name, string[] dataPointTypeNames) =>
				(DataPointTypeNames, Name) = (dataPointTypeNames, name);
	}
}
