using System.Collections.Generic;
using System.IO.Abstractions;
using Newtonsoft.Json;
using TheEyeTether.Data;
using TheEyeTether.Extensions;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Utilities.Hypotheses
{
    public static class HypothesesSaver
    {
        private const string SaveFileName = "Hypotheses.json";


        public static void Save(
                List<Hypothesis> newHypotheses,
                IFileSystem fileSystem,
                ICurrentDomainBaseDirectoryGetter currentDomainBaseDirectoryGetter)
        {
            if(newHypotheses.Count == 0)
            {
                return;
            }
            
            var outputFilePath = GetOutputFilePath(fileSystem, currentDomainBaseDirectoryGetter);
            List<Hypothesis> hypotheses;
            try
            {
                var outputFile = fileSystem.File.ReadAllText(outputFilePath);
                hypotheses = JsonConvert.DeserializeObject<List<Hypothesis>>(outputFile);    
            }
            catch
            {
                hypotheses = new List<Hypothesis>();
            }
            hypotheses.AddUniques(newHypotheses);
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
