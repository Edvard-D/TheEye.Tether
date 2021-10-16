using System.IO.Abstractions;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Utilities.Hypotheses
{
	public static class SnapshotDeleter
	{
		public static void DeleteOutdatedFiles(
				string directoryPath,
				int keepLookbackDays,
				IFileSystem fileSystem,
				IClock clock)
		{
			if(keepLookbackDays <= 0)
			{
				throw new System.InvalidOperationException(string.Format("Argument {0} must be positive.",
						nameof(keepLookbackDays)));
			}

			foreach(string filePath in fileSystem.Directory.GetFiles(directoryPath))
			{
				var creationDateTime = fileSystem.File.GetCreationTimeUtc(filePath);
				var keepThresholdDateTime = clock.Now.ToUniversalTime().AddDays(-keepLookbackDays);

				if(creationDateTime < keepThresholdDateTime)
				{
					fileSystem.File.Delete(filePath);
				}
			}
		}
	}
}
