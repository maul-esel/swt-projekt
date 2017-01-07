using System;
namespace Lingvo.Common.Entities
{
	/// <summary>
	/// A real page.
	/// </summary>
	public class Page : IPage
	{
		private int number;
		private String description;

		private Recording teacherTrack;
		private Recording studentTrack;

		/// <summary>
		/// Gets or sets the page number.
		/// </summary>
		/// <value>The number.</value>
		public int Number
		{
			get
			{
				return number;
			}
			set
			{
				number = value;
			}
		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public String Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Lingvo.Common.Page"/> is edited.
		/// </summary>
		/// <value><c>true</c> if edited; otherwise, <c>false</c>.</value>
		public bool Edited
		{
			get
			{
				return studentTrack != null;
			}
		}

		/// <summary>
		/// Gets or sets the teacher track.
		/// </summary>
		/// <value>The teacher track.</value>
		public Recording TeacherTrack
		{
			get
			{
				return teacherTrack;
			}
			set
			{
				teacherTrack = value;
			}
		}

		/// <summary>
		/// Gets or sets the student track.
		/// </summary>
		/// <value>The student track.</value>
		public Recording StudentTrack
		{
			get
			{
				return studentTrack;
			}
			set
			{
				studentTrack = value;
			}
		}


		public Page()
		{
		}

		/// <summary>
		/// Deletes the student recording.
		/// </summary>
		public void DeleteStudentRecording()
		{
			studentTrack = null;
		}
	}
}
