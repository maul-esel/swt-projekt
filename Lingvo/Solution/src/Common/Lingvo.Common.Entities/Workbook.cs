using System;
using System.Collections.Generic;

namespace Lingvo.Common
{
	public class Workbook
	{

		private int id;
		private string title;
		private string subtitle;
		private bool isPublished;
		private DateTime lastModified;
		private List<Page> pages;


		public int Id
		{
			get
			{
				return id;
			}
		}


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


		public int TotalPages
		{
			get
			{
				return pages.Count;
			}
		}

		public List<Page> Pages
		{
			get
			{
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

		public void deletePage(Page p)
		{
			pages.Remove(p);
		}

		public void savePage(Page p)
		{
			pages.Add(p);
		}


		public Workbook(int id)
		{
			this.id = id;
		}

		public Workbook()
		{
		}
	}
}
