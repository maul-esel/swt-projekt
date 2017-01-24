using System;
using Microsoft.EntityFrameworkCore;
using Lingvo.Common.Entities;

namespace Lingvo.Common.Services
{
	public class LingvoContext: DbContext
	{
		
		public LingvoContext(DbContextOptions<LingvoContext> options)
			: base(options)
		{ }

		public DbSet<Workbook> 		Workbooks { get; set; }
		public DbSet<Page> 			Pages { get; set; }
		public DbSet<Recording> 	Recordings { get; set; }

	}
}
