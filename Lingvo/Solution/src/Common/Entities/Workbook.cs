using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB.Mapping;

using Newtonsoft.Json;

namespace Lingvo.Common.Entities
{
	[Table("Workbooks")]
	public class Workbook : IDownloadable
	{
		/// <summary>
		/// Gets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		[Column, PrimaryKey, NotNull]
		public int Id { get;  set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		[Column, NotNull]
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the subtitle.
		/// </summary>
		/// <value>The subtitle.</value>
		[Column, NotNull]
		public string Subtitle { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Lingvo.Common.Workbook"/> is published.
		/// </summary>
		/// <value><c>true</c> if is published; otherwise, <c>false</c>.</value>
		[Column("published"), NotNull]
		public bool IsPublished { get; set; }

		/// <summary>
		/// Gets or sets the date on which this workbook was modified the last time.
		/// </summary>
		/// <value>The last modified.</value>
		[Column, NotNull]
		public DateTime LastModified { get; set; }

		/// <summary>
		/// Gets or sets all the pages of .
		/// </summary>
		/// <value>The pages.</value>
		
		/******* TODO: Linq2DB polymorphism workaround: ********
		 * 
		 * Linq2DB attempts to create instances of IPage and thus fails. It should be possible to specify the concrete type
		 * (see "OtherType" below). Pending this feature, a workaround is implemented to load the pages on-demand from the DB.
		 * 
		 * */

		// TODO: [Association(ThisKey = nameof(Id), OtherKey = nameof(Page.workbookId), TODO: OtherType = typeof(List<Page>))]
		public List<IPage> Pages // { get; set; }
			=> LoadPages();

		/// <summary>
		/// Gets the total number of pages belonging to this workbook.
		/// </summary>
		/// <value>The total pages.</value>
		[Column, NotNull]
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
			Id = id;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Lingvo.Common.Workbook"/> class.
		/// </summary>
		public Workbook()
		{
		}

		#region Linq2DB polymorphism workaround

		private Services.DatabaseService service;

		internal void SetDatabaseService(Services.DatabaseService service)
		{
			this.service = service;
		}

		private List<IPage> LoadPages()
		{
			if (pages == null)
				pages = service == null ? new List<IPage>() : service.Pages.Where(p => p.workbookId == Id).Cast<IPage>().ToList();
			return pages;
		}

		private List<IPage> pages;

		#endregion
	}
}
