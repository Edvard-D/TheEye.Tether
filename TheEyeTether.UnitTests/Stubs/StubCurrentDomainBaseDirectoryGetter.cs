using TheEyeTether.Interfaces;

namespace TheEyeTether.UnitTests.Stubs
{
    public class StubCurrentDomainBaseDirectoryGetter : ICurrentDomainBaseDirectoryGetter
    {
        private string _baseDirectory;


        public StubCurrentDomainBaseDirectoryGetter(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
        }


        public string GetCurrentDomainBaseDirectory()
        {
            return _baseDirectory;
        }
    }
}
