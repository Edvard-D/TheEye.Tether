using System;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Providers
{
    public class ClockProvider : IClock
    {
        public DateTime Now => DateTime.Now;
    }
}
