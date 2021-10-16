using System;
using TheEye.Tether.Interfaces;

namespace TheEye.Tether.Types
{
	public class Clock : IClock
	{
		public DateTime Now => DateTime.Now;
	}
}
