using System;
using SQLite.Net.Attributes;

namespace Lingvo.Common.Entities
{
	/// <summary>
	/// A page of a workbook.
	/// </summary>
	[Table("Pages")]
	public interface IPage
	{
		[PrimaryKey]
		int Id { get; set; }
        
		/// <summary>
		/// Gets or sets the page number.
		/// </summary>
		/// <value>The number.</value>
		int Number
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		String Description
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the teacher track.
		/// </summary>
		/// <value>The teacher track.</value>
		Recording TeacherTrack
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the student track.
		/// </summary>
		/// <value>The student track.</value>
		Recording StudentTrack
		{
			get;
			set;
		}

		Workbook Workbook { get; set; }

		int workbookId { get; set; }

		/// <summary>
		/// Deletes the student recording that is currently attached to this page.
		/// </summary>
		void DeleteStudentRecording();
	}
}
