using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace Lingvo.Common.Entities
{
	[Table("Workbooks")]
	public class Workbook : IDownloadable
	{
		/// <summary>
		/// Gets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		[PrimaryKey]
		public int Id { get;  set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the subtitle.
		/// </summary>
		/// <value>The subtitle.</value>
		public string Subtitle { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Lingvo.Common.Workbook"/> is published.
		/// </summary>
		/// <value><c>true</c> if is published; otherwise, <c>false</c>.</value>
		public bool IsPublished { get; set; }

		/// <summary>
		/// Gets or sets the date on which this workbook was modified the last time.
		/// </summary>
		/// <value>The last modified.</value>
		public DateTime LastModified { get; set; }

		/// <summary>
		/// Gets or sets all the pages of .
		/// </summary>
		/// <value>The pages.</value>
		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<IPage> Pages  { get; set; }

		/// <summary>
		/// Gets the total number of pages belonging to this workbook.
		/// </summary>
		/// <value>The total pages.</value>
		public int TotalPages { get; set; }

		/// <summary>
		/// Deletes a page from the workbook's collection.
		/// </summary>
		/// <param name="p">Page to be deleted</param>
		public void DeletePage(IPage p)
		{
			Pages.Remove(p);
		}

		/// <summary>
		/// Saves the page.
		/// </summary>
		/// <param name="p">P.</param>
		public void SavePage(IPage p)
		{
			Pages.Add(p);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Lingvo.Common.Workbook"/> class.
		/// </summary>
		/// <param name="id">Unique id</param>
		[JsonConstructor]
		public Workbook(int id)
		{
			Pages = new List<IPage>();			
			Id = id;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Lingvo.Common.Workbook"/> class.
		/// </summary>
		public Workbook()
		{
			Pages = new List<IPage>();
		}

	}
}
