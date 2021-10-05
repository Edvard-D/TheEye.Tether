using System.Collections.Generic;
using System.IO.Abstractions;
using Newtonsoft.Json;
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
            
            var outputFilePath = GetOutputFilePath(fileSystem, currentDomainBaseDirectoryGetter);
            var outputJson = JsonConvert.SerializeObject(hypotheses);
            
            fileSystem.File.WriteAllText(outputFilePath, outputJson);
        }

        private static string GetOutputFilePath(
                IFileSystem fileSystem,
                ICurrentDomainBaseDirectoryGetter currentDomainBaseDirectoryGetter)
        {
            var currentDomainBaseDirectory = currentDomainBaseDirectoryGetter.GetCurrentDomainBaseDirectory();
            var outputFilePathDirectory = fileSystem.Path.Combine(currentDomainBaseDirectory, "Data");
            fileSystem.Directory.CreateDirectory(outputFilePathDirectory);
            var outputFilePath = fileSystem.Path.Combine(outputFilePathDirectory, SaveFileName);

            return outputFilePath;
        }
    }
}