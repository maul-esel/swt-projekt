using System;
using LinqToDB.Mapping;

namespace Lingvo.Common
{
	[Table("Pages")]
	public class Page
	{
		[Column("teacherTrack"), NotNull]
		public int teacherTrackId;

		[Column("studentTrack"), Nullable]
		public int studentTackId;

		[Column, NotNull]
		public int workbookId { get; set; }

		[Column, NotNull]
		public int Number { get; set; }

		[Column, NotNull]
		public String Description { get; set; }

		public bool Edited => StudentTrack != null;

		[Association(ThisKey = nameof(workbookId), OtherKey = nameof(Common.Workbook.Id), CanBeNull = false)]
		public Workbook Workbook { get; set; }

		[Association(ThisKey = nameof(teacherTrackId), OtherKey = nameof(Recording.Id), CanBeNull = false)]
		public Recording TeacherTrack { get; set; }

		[Association(ThisKey = nameof(studentTackId), OtherKey = nameof(Recording.Id), CanBeNull = true)]
		public Recording StudentTrack { get; set; }

		public void DeleteStudentRecording()
		{
			StudentTrack = null;
		}
	}
}