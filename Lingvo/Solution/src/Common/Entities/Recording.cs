using System;

namespace Lingvo.Common.Entities
{
	/// <summary>
	/// Objects from this class represent audio files.
	/// </summary>
	public class Recording
	{
		// to do: save the mp3 file

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Lingvo.Common.Recording"/> class
		/// </summary>
		private Recording() { } // used by Linq2DB

		public Recording(int id, TimeSpan length, string localPath, DateTime creationTime)
		{
			Id = id;
			Length = length;
			LocalPath = localPath;
			CreationTime = creationTime;
		}

		public int Id { get; private set; }

		public DateTime CreationTime { get; private set; }

		/// <summary>
		/// Gets or sets the length.
		/// </summary>
		/// <value>The length.</value>
		public TimeSpan Length { get; set; }


		public string LocalPath { get; set; }
	}
}
