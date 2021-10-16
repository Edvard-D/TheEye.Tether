using System;
using TheEyeTether.Interfaces;

namespace TheEyeTether.Types
{
	public class Clock : IClock
	{
		public DateTime Now => DateTime.Now;
	}
}
