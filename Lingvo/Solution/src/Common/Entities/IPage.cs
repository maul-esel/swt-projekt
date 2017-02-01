using System;
using SQLite.Net.Attributes;

namespace Lingvo.Common.Entities
{
	/// <summary>
	/// A page of a workbook.
	/// </summary>
	[Table("Pages")]
	public interface IPage : IExercisable
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

		Workbook Workbook { get; set; }

		int workbookId { get; set; }
	}
}
