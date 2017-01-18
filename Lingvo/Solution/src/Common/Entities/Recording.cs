using System;
using LinqToDB.Mapping;

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

		public Recording(int id, TimeSpan length, string localPath, DateTime creationTime)
		{
			Id = id;
			Length = length;
			LocalPath = localPath;
			CreationTime = creationTime;
		}

		[Column, PrimaryKey, NotNull]
		public int Id { get; set; }

		[Column, NotNull]
		public DateTime CreationTime { get; private set; }

		/// <summary>
		/// Gets or sets the length.
		/// </summary>
		/// <value>The length.</value>
		[Column, NotNull, DataType(LinqToDB.DataType.Int32)]
		public TimeSpan Length { get; set; }

		[Column, NotNull]
		public string LocalPath { get; set; }
	}
}
