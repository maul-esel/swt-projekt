using System;
namespace Lingvo.Common
{
	public class Page
	{
		private int number;
		private String description;

		private Recording teacherTrack;
		private Recording studentTrack;

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

		public bool Edited
		{
			get
			{
				return studentTrack != null;
			}
		}

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

		public void DeleteStudentRecording()
		{
			studentTrack = null;
		}
	}
}
