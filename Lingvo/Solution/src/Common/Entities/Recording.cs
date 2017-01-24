using System;
using LinqToDB.Mapping;
using System.ComponentModel;

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

		[Column, PrimaryKey, NotNull]
		public int Id { get; set; }

		[Column, NotNull]
		public DateTime CreationTime { get; private set; }

		/// <summary>
		/// Gets or sets the duration of the audio file in milliseconds.
		/// </summary>
		/// <value>The length.</value>
		[Column, NotNull]
		public int Duration { get; set; }

		[Column, NotNull]
		public string LocalPath { get; set; }
	}
}
