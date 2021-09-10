using System.Runtime.InteropServices;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Helpers
{
    public static class OSVersionHelpers
    {
        private const string MacOSProgramEnding = ".app";
        private const string WindowsProgramEnding = ".exe";


        public static string GetProgramEnding(IOSPlatformChecker osPlatformChecker)
        {
            if(osPlatformChecker.IsOSPlatform(OSPlatform.Windows))
            {
                return WindowsProgramEnding;
            }
            else if(osPlatformChecker.IsOSPlatform(OSPlatform.OSX))
            {
                return MacOSProgramEnding;
            }

            throw new System.NotImplementedException("No case has been created for current OSPlatform.");
        }
    }    
}
