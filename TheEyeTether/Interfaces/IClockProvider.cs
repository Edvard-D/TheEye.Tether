using System;

namespace TheEyeTether.Interfaces
{
    public interface IClockProvider
    {
        DateTime Now { get; }
    }
}
