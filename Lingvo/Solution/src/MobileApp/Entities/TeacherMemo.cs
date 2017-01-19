using Lingvo.Common.Entities;
using System;
using LinqToDB.Mapping;

namespace Lingvo.MobileApp.Entities
{
    /// <summary>
    /// Teacher memo: teacher can record a scentence or word the student cant pronounce easily.
    /// </summary>
	[Table("TeacherMemos")]
    public class TeacherMemo
	{
		[Column("recording"), NotNull]
		public String RecordingId { get; set; }

		[Column, NotNull]
		public String Name { get; set; }

		[Column, Identity, PrimaryKey]
		public int Id { get; set; }

		[Association(ThisKey = nameof(RecordingId), OtherKey = nameof(Common.Entities.Recording.Id), CanBeNull = false)]
		public Recording Recording { get; set; }

		public TeacherMemo()
		{
		}
	}
}
