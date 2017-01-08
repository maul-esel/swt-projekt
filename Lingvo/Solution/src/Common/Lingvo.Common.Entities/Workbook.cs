using System;
using System.Collections.Generic;
using LinqToDB.Mapping;

namespace Lingvo.Common
{
	[Table("Workbooks")]
	public class Workbook
	{
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
		public List<Page> Pages { get; set; }

		public int TotalPages => Pages.Count;

		public void DeletePage(Page p)
		{
			// TODO: delete page from db?
			Pages.Remove(p);
		}

		public void SavePage(Page p)
		{
			// TODO: save page in db?
			Pages.Add(p);
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
