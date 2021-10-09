using System;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Providers
{
    public class CurrentDomainBaseDirectoryProvider : ICurrentDomainBaseDirectoryProvider
    {
        public string GetCurrentDomainBaseDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
