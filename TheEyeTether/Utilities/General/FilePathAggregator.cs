using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace TheEye.Tether.Utilities.General
{
	public static class FilePathAggregator
	{
		private const string TempFileName = "TheEyeTether_TempFile.txt";

		
		public static List<string> Aggregate(
				string searchDirectory,
				string requiredDirectories,
				string fileName,
				IFileSystem fileSystem)
		{
			var directoryInfo = fileSystem.DirectoryInfo.FromDirectoryName(searchDirectory);

			return AggregateRecursively(directoryInfo, requiredDirectories, fileName, fileSystem);
		}

		private static List<string> AggregateRecursively(
				IDirectoryInfo searchDirectoryInfo,
				string requiredDirectories,
				string fileName,
				IFileSystem fileSystem,
				List<string> foundFiles = null)
		{
			if(foundFiles == null)
			{
				foundFiles = new List<string>();
			}

			try
			{
				// We attempt to create a file to test whether or not we have write access.
				var tempPath = Path.Combine(searchDirectoryInfo.FullName, TempFileName);
				fileSystem.File.WriteAllText(tempPath, string.Empty);
				fileSystem.File.Delete(tempPath);
			}
			catch(System.UnauthorizedAccessException)
			{
				return foundFiles;
			}
			catch(System.IO.IOException)
			{
				return foundFiles;
			}

			var subDirectories = searchDirectoryInfo.EnumerateDirectories();
			foreach(var subDirectory in subDirectories)
			{
				foundFiles = AggregateRecursively(subDirectory, requiredDirectories,
						fileName, fileSystem, foundFiles);
			}
			
			var files = searchDirectoryInfo.GetFiles();
			var filteredFiles = files
					.Where(f => f.FullName.Contains(requiredDirectories))
					.Where(f => f.Name == fileName)
					.Select(f => f.FullName)
					.ToList();
			foundFiles.AddRange(filteredFiles);

			return foundFiles;
		}
	}
}