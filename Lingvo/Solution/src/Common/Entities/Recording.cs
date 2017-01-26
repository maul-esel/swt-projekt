using System;
namespace Lingvo.Common.Entities
{
	/// <summary>
	/// Objects from this class represent audio files.
	/// </summary>
	public class Recording
	{
		public int Id { get; private set; }

		public DateTime CreationTime { get; private set; }

		/// <summary>
		/// Gets or sets the duration of the audio file in milliseconds.
		/// </summary>
		/// <value>The length.</value>
		public int Duration { get; set; }

		public string LocalPath { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Lingvo.Common.Recording"/> class, the creation time is set
		/// to the moment the constructor is called.
		/// </summary>
		public Recording()
		{
			CreationTime = DateTime.Now;
		}

		public Recording(int id, int duration, string localPath, DateTime creationTime)
		{
			Id = id;
			Duration = duration;
			LocalPath = localPath;
			CreationTime = creationTime;
		}

	}
}
