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
		/// Gets or sets the length.
		/// </summary>
		/// <value>The length.</value>
		public TimeSpan Length { get; private set; }

		public string LocalPath { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Lingvo.Common.Recording"/> class, the creation time is set
		/// to the moment the constructor is called.
		/// </summary>
		public Recording()
		{
			CreationTime = DateTime.Now;
		}

		public Recording(int id, TimeSpan length, string localPath, DateTime creationTime)
		{
			Id = id;
			Length = length;
			LocalPath = localPath;
			CreationTime = creationTime;
		}

	}
}
