using System;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Providers
{
    public class ClockProvider : IClockProvider
    {
        public DateTime Now => DateTime.Now;
    }
}
