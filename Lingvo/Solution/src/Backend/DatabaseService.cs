using System;

using Microsoft.Extensions.Configuration;

using Lingvo.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Server.Kestrel;
using System.Collections.Generic;

namespace Lingvo.Backend
{
	using Common.Entities;

	public class DatabaseService : DbContext
    {

		public DbSet<Workbook> Workbooks { get; set; }
		public DbSet<Page> Pages { get; set; }
		public DbSet<Recording> Recordings { get; set; }

		public DatabaseService(DbContextOptions<DatabaseService> options) : base(options)
    	{ }

		public static DatabaseService Connect(IConfiguration config)
		{
			var optionsBuilder = new DbContextOptionsBuilder<DatabaseService>();
			var server = config["DB_HOST"];
			var port = config["DB_PORT"];
			var db = config["DB_NAME"];
			var user = config["DB_USER"];
			var password = config["DB_PASSWORD"];

			optionsBuilder.UseMySql(@"Server=" + server + ";port=" + port + ";database=" + db + ";uid=" + user + ";pwd=" + password + ";");

			return new DatabaseService( optionsBuilder.Options );
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Workbook>().Property(w => w.Title).IsRequired();
			modelBuilder.Entity<Workbook>().Property(w => w.Subtitle).IsRequired();
			modelBuilder.Entity<Workbook>().Property(w => w.LastModified).IsRequired().ValueGeneratedOnAddOrUpdate();
			modelBuilder.Entity<Workbook>().HasMany(w => (IEnumerable<Page>) w.Pages).WithOne(p => p.Workbook).HasForeignKey(p => p.workbookId);

			modelBuilder.Entity<Page>().Property(p => p.Description).IsRequired();
			modelBuilder.Entity<Page>().HasOne(p => p.StudentTrack).WithMany().HasForeignKey(p => p.studentTrackId);
			modelBuilder.Entity<Page>().HasOne(p => p.TeacherTrack).WithMany().HasForeignKey(p => p.teacherTrackId);

			modelBuilder.Entity<Recording>().Property(r => r.LocalPath).IsRequired();
		}

		/// <summary>
		/// Save the specified recording, updates it if it already exists.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="recording">Recording.</param>
		public void Save(Recording recording)
		{
			
		}

		/// <summary>
		/// Save the specified page, updates it if it already exists.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="page">Page.</param>
		public void Save(Page page)
		{
			
		}

		/// <summary>
		/// Save the specified workbook, updates it if it already exists.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="workbook">Workbook.</param>
		public void Save(Workbook workbook)
		{
			
		}
    }
}
