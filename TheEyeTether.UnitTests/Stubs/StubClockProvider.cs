using System;
using TheEyeTether.Interfaces;

namespace TheEyeTether.UnitTests.Stubs
{
    public class StubClock : IClockProvider
    {
        private DateTime _now;

        
        public StubClock(DateTime now)
        {
            _now = now;
        }


        public DateTime Now { get { return _now; } }
    }
}
