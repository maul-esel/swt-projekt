using System;
namespace Lingvo.Common
{
	public class Recording
	{
		private TimeSpan length;
		private DateTime creationTime;

		public TimeSpan Length
		{
			get
			{
				return length;
			}
			set
			{
				length = value;
			}
		}

		public Recording()
		{
			creationTime = DateTime.Now;
		}
	}
}
