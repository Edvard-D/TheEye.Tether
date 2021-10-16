using System.Collections.Generic;

namespace TheEye.Tether.Data
{
	public record CategorySetting
	{
		public string Name;
		public Dictionary<string, SnapshotSetting> SnapshotSettings;


		public CategorySetting(string name, Dictionary<string, SnapshotSetting> snapshotSettings) =>
				(Name, SnapshotSettings) = (name, snapshotSettings);
	}
}
