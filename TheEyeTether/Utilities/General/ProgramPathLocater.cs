using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using TheEye.Tether.Interfaces;

namespace TheEye.Tether.Utilities.General
{
	public static class ProgramPathLocater
	{
		private static Dictionary<string, string> _savedProgramPathPairs = new Dictionary<string, string>();


		public static Dictionary<string, string> SavedProgramPathPairs { get { return _savedProgramPathPairs;  } }

		
		/// It's recommended to pass programName without a file ending as this will add the appropriate one
		/// based on the operating system being used.
		public static string LocateProgramPath(
				string programName,
				string requiredDirectories,
				IFileSystem fileSystem,
				IDrivesProvider drivesProvider,
				IOSPlatformChecker osPlatformChecker,
				string defaultPath = null)
		{
			if(_savedProgramPathPairs.ContainsKey(programName) == true
					&& fileSystem.File.Exists(_savedProgramPathPairs[programName]) == true)
			{
				return _savedProgramPathPairs[programName];
			}

			var ending = GetAppropriateFileEnding(programName, osPlatformChecker);
			var searchPattern = programName + ending;
			var files = LocateFiles(requiredDirectories, searchPattern, defaultPath, fileSystem,
					drivesProvider);

			if(files.Count == 0)
			{
				return null;
			}

			var programPath = files[0];
			_savedProgramPathPairs[programName] = programPath;

			return programPath;
		}

		private static string GetAppropriateFileEnding(
				string programName,
				IOSPlatformChecker osPlatformChecker)
		{
			var ending = OSPlatformUtilities.GetProgramEnding(osPlatformChecker);

			if(programName.Contains(ending))
			{
				return string.Empty;
			}

			return ending;
		}

		private static List<string> LocateFiles(
				string requiredDirectories,
				string searchPattern,
				string defaultPath,
				IFileSystem fileSystem,
				IDrivesProvider drivesProvider)
		{
			if(defaultPath != null)
			{
				return FilePathAggregator.Aggregate(defaultPath, requiredDirectories,
						searchPattern, fileSystem);
			}

			foreach(DriveInfo driveInfo in drivesProvider.GetDrives())
			{
				var files = FilePathAggregator.Aggregate(driveInfo.Name, requiredDirectories, searchPattern,
						fileSystem);

				if(files.Count > 0)
				{
					return files;
				}
			}

			return new List<string>();
		}
	}
}
