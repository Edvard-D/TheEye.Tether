using System.Runtime.InteropServices;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Helpers
{
    public static class OSVersionHelpers
    {
        private const string WindowsProgramEnding = ".exe";


        public static string GetProgramEnding(IOSPlatformChecker osPlatformChecker)
        {
            if(osPlatformChecker.IsOSPlatform(OSPlatform.Windows))
            {
                return WindowsProgramEnding;
            }

            throw new System.NotImplementedException("No case has been created for current OSPlatform.");
        }
    }    
}
