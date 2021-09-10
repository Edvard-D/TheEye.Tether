using TheEyeTether.Interfaces;

namespace TheEyeTether.UnitTests.Stubs
{
    public class CurrentDomainBaseDirectoryGetter : ICurrentDomainBaseDirectoryGetter
    {
        private string _baseDirectory;


        public CurrentDomainBaseDirectoryGetter(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
        }


        public string GetCurrentDomainBaseDirectory()
        {
            return _baseDirectory;
        }
    }
}
