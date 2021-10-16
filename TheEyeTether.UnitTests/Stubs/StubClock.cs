using System;
using TheEye.Tether.Interfaces;

namespace TheEye.Tether.UnitTests.Stubs
{
	public class StubClock : IClock
	{
		private DateTime _now;

		
		public StubClock(DateTime now)
		{
			_now = now;
		}


		public DateTime Now { get { return _now; } }
	}
}
