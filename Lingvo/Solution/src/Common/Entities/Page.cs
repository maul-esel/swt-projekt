using System;

namespace Lingvo.Common.Entities
{
	/// <summary>
	/// A real page.
	/// </summary>
	public class Page : IPage
	{
		public int teacherTrackId;

		public int studentTackId;

		public int workbookId { get; set; }

		/// <summary>
		/// Gets or sets the page number.
		/// </summary>
		/// <value>The number.</value>
		public int Number { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public String Description { get; set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Lingvo.Common.Page"/> is edited.
		/// </summary>
		/// <value><c>true</c> if edited; otherwise, <c>false</c>.</value>
		public bool Edited => StudentTrack != null;

		public Workbook Workbook { get; set; }

		/// <summary>
		/// Gets or sets the teacher track.
		/// </summary>
		/// <value>The teacher track.</value>
		public Recording TeacherTrack { get; set; }

		/// <summary>
		/// Gets or sets the student track.
		/// </summary>
		/// <value>The student track.</value>
		public Recording StudentTrack { get; set; }

		public void DeleteStudentRecording()
		{
			StudentTrack = null;
		}
	}
}