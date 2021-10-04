using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using TheEyeTether.Types;
using TheEyeTether.UnitTests.Stubs;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Types
{
    public class HypothesesSaverTests
    {
        private const string ProgramPath = @"C:\TestProgram\";
        private const string SavePath = ProgramPath + @"Data\Hypotheses.json";


        [Fact]
        public void Save_CreatesFile_WhenItDoesNotExistAndThereAreHypothesesToSave()
        {
            var dataPointStrings = new HashSet<string>() { "testDataPointString" };
            var hypotheses = new List<Hypothesis>() { new Hypothesis(dataPointStrings) };
            var mockFileSystem = new MockFileSystem();
            var stubCurrentDomainBaseDirectoryGetter = new StubCurrentDomainBaseDirectoryGetter(ProgramPath);

            HypothesesSaver.Save(hypotheses, mockFileSystem, stubCurrentDomainBaseDirectoryGetter);

            Assert.Contains(SavePath, mockFileSystem.AllFiles);
        }
    }
}
