using System;
using TheEye.Tether.Interfaces;

namespace TheEye.Tether.Providers
{
	public class CurrentDomainBaseDirectoryProvider : ICurrentDomainBaseDirectoryProvider
	{
		public string GetCurrentDomainBaseDirectory()
		{
			return AppDomain.CurrentDomain.BaseDirectory;
		}
	}
}
