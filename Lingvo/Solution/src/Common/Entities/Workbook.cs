using System;
using System.Collections.Generic;

namespace Lingvo.Common.Entities
{
	public class Workbook
	{

		private int id;
		private string title;
		private string subtitle;
		private bool isPublished;
		private DateTime lastModified;
		private List<Page> pages;

		/// <summary>
		/// Gets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public int Id
		{
			get
			{
				return id;
			}
		}

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title
		{
			get
			{
				return title;
			}

			set
			{
				title = value;
			}
				
		}

		/// <summary>
		/// Gets or sets the subtitle.
		/// </summary>
		/// <value>The subtitle.</value>
		public string Subtitle
		{
			get
			{
				return subtitle;
			}

			set
			{
				subtitle = value;
			}

		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Lingvo.Common.Workbook"/> is published.
		/// </summary>
		/// <value><c>true</c> if is published; otherwise, <c>false</c>.</value>
		public bool IsPublished
		{
			get
			{
				return isPublished;
			}
			set
			{
				isPublished = value;
			}
		}

		/// <summary>
		/// Gets or sets the date on which this workbook was modified the last time.
		/// </summary>
		/// <value>The last modified.</value>
		public DateTime LastModified
		{
			get
			{
				return lastModified;
			}

			set
			{
				lastModified = value;
			}
				
		}

		/// <summary>
		/// Gets the total number of pages belonging to this workbook.
		/// </summary>
		/// <value>The total pages.</value>
		public int TotalPages
		{
			get
			{
				return pages.Count;
			}
		}


		/// <summary>
		/// Gets or sets all the pages of .
		/// </summary>
		/// <value>The pages.</value>
		public List<Page> Pages
		{
			get
			{
				//Nullpointer avoidance
				if (pages == null)
				{
					pages = new List<Page>();
				}

				return pages;
			}

			set
			{
				pages = value;
			}

				
		}

		/// <summary>
		/// Deletes a page from the workbook's collection.
		/// </summary>
		/// <param name="p">Page to be deleted</param>
		public void DeletePage(Page p)
		{
			pages.Remove(p);
		}


		/// <summary>
		/// Saves the page.
		/// </summary>
		/// <param name="p">P.</param>
		public void SavePage(Page p)
		{
			pages.Add(p);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Lingvo.Common.Workbook"/> class.
		/// </summary>
		/// <param name="id">Unique id</param>
		public Workbook(int id)
		{
			this.id = id;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Lingvo.Common.Workbook"/> class.
		/// </summary>
		public Workbook()
		{
		}
	}
}
