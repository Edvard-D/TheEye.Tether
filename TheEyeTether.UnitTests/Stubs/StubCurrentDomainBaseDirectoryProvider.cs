using TheEye.Tether.Interfaces;

namespace TheEye.Tether.UnitTests.Stubs
{
	public class StubCurrentDomainBaseDirectoryProvider : ICurrentDomainBaseDirectoryProvider
	{
		private string _baseDirectory;


		public StubCurrentDomainBaseDirectoryProvider(string baseDirectory)
		{
			_baseDirectory = baseDirectory;
		}


		public string GetCurrentDomainBaseDirectory()
		{
			return _baseDirectory;
		}
	}
}
