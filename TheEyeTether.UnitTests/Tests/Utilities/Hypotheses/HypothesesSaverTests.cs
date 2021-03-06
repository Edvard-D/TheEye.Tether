using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Newtonsoft.Json;
using TheEyeTether.Data;
using TheEyeTether.UnitTests.Stubs;
using TheEyeTether.Utilities.Hypotheses;
using Xunit;

namespace TheEyeTether.UnitTests.Tests.Utilities.Hypotheses
{
    public class HypothesesSaverTests
    {
        private const string ProgramPath = @"C:\TestProgram\";
        private const string OutputFilePath = ProgramPath + @"Data\Hypotheses.json";


        [Fact]
        public void Save_CreatesFile_WhenItDoesNotExistAndThereAreHypothesesToSave()
        {
            var dataPointStrings = new HashSet<string>() { "testDataPointString" };
            var hypotheses = new List<Hypothesis>() { new Hypothesis(dataPointStrings) };
            var mockFileSystem = new MockFileSystem();
            var stubCurrentDomainBaseDirectoryProvider = new StubCurrentDomainBaseDirectoryProvider(
                    ProgramPath);

            HypothesesSaver.Save(hypotheses, mockFileSystem, stubCurrentDomainBaseDirectoryProvider);

            Assert.Contains(OutputFilePath, mockFileSystem.AllFiles);
        }

        [Fact]
        public void Save_DoesNotCreateFile_WhenItDoesNotExistAndThereAreNoHypothesesToSave()
        {
            var hypotheses = new List<Hypothesis>();
            var mockFileSystem = new MockFileSystem();
            var stubCurrentDomainBaseDirectoryProvider = new StubCurrentDomainBaseDirectoryProvider(
                    ProgramPath);

            HypothesesSaver.Save(hypotheses, mockFileSystem, stubCurrentDomainBaseDirectoryProvider);

            Assert.DoesNotContain(OutputFilePath, mockFileSystem.AllFiles);
        }

        [Fact]
        public void Save_SerializesDataAsJsonListOfHypotheses_WhenCalled()
        {
            var categoryType = "testCategoryType";
            var categoryId = "testCategoryId";
            var testDataPointString = "testDataPointString";
            var dataPointStrings = new HashSet<string>() { testDataPointString };
            var snapshotType = "testSnapshotType";
            var snapshotId = "testSnapshotId";
            var hypotheses = new List<Hypothesis>() { new Hypothesis(categoryType, categoryId,
                    snapshotType, snapshotId, dataPointStrings) };
            var mockFileSystem = new MockFileSystem();
            var stubCurrentDomainBaseDirectoryProvider = new StubCurrentDomainBaseDirectoryProvider(
                    ProgramPath);

            HypothesesSaver.Save(hypotheses, mockFileSystem, stubCurrentDomainBaseDirectoryProvider);

            var outputFile = mockFileSystem.File.ReadAllText(OutputFilePath);
            var outputJson = JsonConvert.DeserializeObject<List<Hypothesis>>(outputFile);
            var hypothesis = outputJson[0];
            Assert.Equal(categoryType, hypothesis.CategoryType);
            Assert.Equal(categoryId, hypothesis.CategoryId);
            Assert.Contains(testDataPointString, hypothesis.DataPointStrings);
            Assert.Equal(DateTime.UnixEpoch, hypothesis.SentDateTime);
            Assert.Equal(snapshotType, hypothesis.SnapshotType);
            Assert.Equal(snapshotId, hypothesis.SnapshotId);
            Assert.False(hypothesis.WasSent);
        }

        [Fact]
        public void Save_DoesNotErasePreviouslySavedHypotheses_WhenCalled()
        {
            var categoryType = "testCategoryType";
            var categoryId = "testCategoryId";
            var testDataPointString = "testDataPointString";
            var dataPointStrings = new HashSet<string>() { testDataPointString };
            var snapshotType = "testSnapshotType";
            var snapshotId = "testSnapshotId";
            var nowDateTime = DateTime.UtcNow;
            var testHypothesis = new Hypothesis(categoryType, categoryId, snapshotType, snapshotId,
                    dataPointStrings, nowDateTime, true);
            var inputHypothesis = new Hypothesis(categoryType, categoryId, snapshotType, snapshotId,
                    dataPointStrings);
            var hypotheses = new List<Hypothesis>() { inputHypothesis };
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { OutputFilePath, new MockFileData(JsonConvert.SerializeObject(new List<Hypothesis>()
                        { testHypothesis })) }
            });
            var stubCurrentDomainBaseDirectoryProvider = new StubCurrentDomainBaseDirectoryProvider(
                    ProgramPath);
            
            HypothesesSaver.Save(hypotheses, mockFileSystem, stubCurrentDomainBaseDirectoryProvider);
            
            var outputFile = mockFileSystem.File.ReadAllText(OutputFilePath);
            var outputJson = JsonConvert.DeserializeObject<List<Hypothesis>>(outputFile);
            var hypothesis = outputJson[0];
            Assert.Equal(nowDateTime, hypothesis.SentDateTime);
            Assert.True(hypothesis.WasSent);
        }

        [Fact]
        public void Save_DoesNotThrowException_WhenCalledWithNoExistingOutputFile()
        {
            var dataPointStrings = new HashSet<string>() { "testDataPointString" };
            var hypotheses = new List<Hypothesis>() { new Hypothesis(dataPointStrings) };
            var mockFileSystem = new MockFileSystem();
            var stubCurrentDomainBaseDirectoryProvider = new StubCurrentDomainBaseDirectoryProvider(
                    ProgramPath);

            try
            {
                HypothesesSaver.Save(hypotheses, mockFileSystem, stubCurrentDomainBaseDirectoryProvider);
                Assert.True(true);
            }
            catch
            {
                Assert.True(false);
            }
        }
    }
}
