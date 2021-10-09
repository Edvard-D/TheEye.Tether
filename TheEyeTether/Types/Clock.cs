using System;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Types
{
    public class ClockProvider : IClock
    {
        public DateTime Now => DateTime.Now;
    }
}
