using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Utilities.General
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
			var searchPattern = "*" + requiredDirectories + programName + ending;
			var files = LocateFiles(searchPattern, defaultPath, fileSystem, drivesProvider);

			if(files.Length == 0)
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

		private static string[] LocateFiles(
				string searchPattern,
				string defaultPath,
				IFileSystem fileSystem,
				IDrivesProvider drivesProvider)
		{
			if(defaultPath != null)
			{
				var files = fileSystem.Directory.GetFiles(defaultPath, searchPattern);

				if(files.Length > 0)
				{
					return files;
				}
			}

			foreach(DriveInfo driveInfo in drivesProvider.GetDrives())
			{
				var files = fileSystem.Directory.GetFiles(driveInfo.Name, searchPattern, SearchOption.AllDirectories);
				
				if(files.Length > 0)
				{
					return files;
				}
			}

			return new string[0];
		}
	}
}
