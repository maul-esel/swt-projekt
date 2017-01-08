using System;
using System.Collections.Generic;
using LinqToDB.Mapping;

namespace Lingvo.Common
{
	[Table("Workbooks")]
	public class Workbook
	{
		private List<Page> pages;

		[Column, PrimaryKey]
		public int Id { get; private set; }

		[Column, NotNull]
		public string Title { get; set; }

		[Column, NotNull]
		public string Subtitle { get; set; }

		[Column("published"), NotNull]
		public bool IsPublished { get; set; }

		[Column, NotNull]
		public DateTime LastModified { get; set; }

		[Association(ThisKey = nameof(Id), OtherKey = nameof(Page.workbookId))]
		public List<Page> Pages2 { get; set; }

		public int TotalPages => pages.Count;

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

		public void DeletePage(Page p)
		{
			pages.Remove(p);
		}

		public void SavePage(Page p)
		{
			pages.Add(p);
		}

		public Workbook(int id)
		{
			Id = id;
		}

		public Workbook()
		{
		}
	}
}
