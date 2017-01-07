using System;
namespace Lingvo.Common.Entities
{
	/// <summary>
	/// Objects from this class represent audio files.
	/// </summary>
	public class Recording
	{
		private TimeSpan length;
		private DateTime creationTime;
		// to do: save the mp3 file

		/// <summary>
		/// Gets or sets the length.
		/// </summary>
		/// <value>The length.</value>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Lingvo.Common.Recording"/> class, the creation time is set
		/// to the moment the constructor is called.
		/// </summary>
		public Recording()
		{
			creationTime = DateTime.Now;
		}
	}
}
