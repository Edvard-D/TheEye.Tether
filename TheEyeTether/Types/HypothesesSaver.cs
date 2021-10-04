using System.Collections.Generic;
using System.IO.Abstractions;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Types
{
    public static class HypothesesSaver
    {
        private const string SaveFileName = "Hypotheses.json";


        public static void Save(
                List<Hypothesis> hypotheses,
                IFileSystem fileSystem,
                ICurrentDomainBaseDirectoryGetter currentDomainBaseDirectoryGetter)
        {
            if(hypotheses.Count == 0)
            {
                return;
            }

            var currentDomainBaseDirectory = currentDomainBaseDirectoryGetter.GetCurrentDomainBaseDirectory();
            var outputFilePathDirectory = fileSystem.Path.Combine(currentDomainBaseDirectory, "Data");
            fileSystem.Directory.CreateDirectory(outputFilePathDirectory);
            var outputFilePath = fileSystem.Path.Combine(outputFilePathDirectory, SaveFileName);

            fileSystem.File.WriteAllText(outputFilePath, string.Empty);
        }
    }
}
