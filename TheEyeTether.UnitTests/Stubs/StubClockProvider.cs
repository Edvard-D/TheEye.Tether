using System;
using TheEyeTether.Interfaces;

namespace TheEyeTether.UnitTests.Stubs
{
    public class StubClockProvider : IClockProvider
    {
        private DateTime _now;

        
        public StubClockProvider(DateTime now)
        {
            _now = now;
        }


        public DateTime Now { get { return _now; } }
    }
}
