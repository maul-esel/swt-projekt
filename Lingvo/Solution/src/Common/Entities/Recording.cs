using System;
using SQLite.Net.Attributes;

namespace Lingvo.Common.Entities
{
	/// <summary>
	/// Objects from this class represent audio files.
	/// </summary>
	[Table("Recordings")]
	public class Recording
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Lingvo.Common.Recording"/> class
		/// </summary>
		public Recording() { } // used by Linq2DB

		public Recording(int id, int duration, string localPath, DateTime creationTime)
		{
			Id = id;
			Duration = duration;
			LocalPath = localPath;
			CreationTime = creationTime;
		}

		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public DateTime CreationTime { get; private set; }

		/// <summary>
		/// Gets or sets the duration of the audio file in milliseconds.
		/// </summary>
		/// <value>The length.</value>
		public int Duration { get; set; }

		public string LocalPath { get; set; }
	}
}
