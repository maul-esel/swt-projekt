using System;
using LinqToDB.Mapping;

namespace Lingvo.Common
{
	[Table("Pages")]
	public class Page
	{
		[Column, NotNull]
		public int Number { get; set; }

		[Column, NotNull]
		public String Description { get; set; }

		public bool Edited => StudentTrack != null;

		[Association(ThisKey = nameof(Workbook), OtherKey = nameof(Common.Workbook.Id), CanBeNull = false)]
		public Workbook Workbook { get; set; }

		[Association(ThisKey = nameof(TeacherTrack), OtherKey = nameof(Recording.Id), CanBeNull = false)]
		public Recording TeacherTrack { get; set; }

		[Association(ThisKey = nameof(StudentTrack), OtherKey = nameof(Recording.Id), CanBeNull = true)]
		public Recording StudentTrack { get; set; }

		public void DeleteStudentRecording()
		{
			StudentTrack = null;
		}
	}
}