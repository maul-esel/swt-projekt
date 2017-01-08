using System;
using LinqToDB.Mapping;

namespace Lingvo.Common
{
	/// <summary>
	/// A real page.
	/// </summary>
	[Table("Pages")]
	public class Page
	{
		[Column("teacherTrack"), NotNull]
		public int teacherTrackId;

		[Column("studentTrack"), Nullable]
		public int studentTackId;

		[Column, NotNull]
		public int workbookId { get; set; }

		/// <summary>
		/// Gets or sets the page number.
		/// </summary>
		/// <value>The number.</value>
		[Column, NotNull]
		public int Number { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		[Column, NotNull]
		public String Description { get; set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Lingvo.Common.Page"/> is edited.
		/// </summary>
		/// <value><c>true</c> if edited; otherwise, <c>false</c>.</value>
		public bool Edited => StudentTrack != null;

		[Association(ThisKey = nameof(workbookId), OtherKey = nameof(Common.Workbook.Id), CanBeNull = false)]
		public Workbook Workbook { get; set; }

		/// <summary>
		/// Gets or sets the teacher track.
		/// </summary>
		/// <value>The teacher track.</value>
		[Association(ThisKey = nameof(teacherTrackId), OtherKey = nameof(Recording.Id), CanBeNull = false)]
		public Recording TeacherTrack { get; set; }

		/// <summary>
		/// Gets or sets the student track.
		/// </summary>
		/// <value>The student track.</value>
		[Association(ThisKey = nameof(studentTackId), OtherKey = nameof(Recording.Id), CanBeNull = true)]
		public Recording StudentTrack { get; set; }

		public void DeleteStudentRecording()
		{
			StudentTrack = null;
		}
	}
}