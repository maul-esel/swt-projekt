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
		public TimeSpan Length
		{
			get { return TimeSpan.FromMilliseconds(LengthInMilliseconds); }
			set { LengthInMilliseconds = value.Milliseconds; }
		}

		/// <summary>
		/// Workaround: due to a bug in linq2db, <see cref="TimeSpan"/>s cannot be converted to <c>int</c>s (for MySQL).
		/// Thus this additional property is needed. It should never be used in code.
		/// </summary>
		[Column("length"), NotNull, EditorBrowsable(EditorBrowsableState.Never)]
		public int LengthInMilliseconds { get; set; } // HACK: avoid this workaround if possible

		[Column, NotNull]
		public string LocalPath { get; set; }
	}
}
