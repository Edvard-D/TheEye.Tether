using System.IO;
using System.IO.Abstractions;

namespace TheEye.Tether.Utilities.General
{
	public static class FilePathAggregator
	{
		public static string[] AggregateFilePaths(
				string fileName,
				string searchDirectoryPath,
				IFileSystem fileSystem)
		{
			return fileSystem.Directory.GetFiles(searchDirectoryPath, fileName,
					SearchOption.AllDirectories);
		}
	}
}
