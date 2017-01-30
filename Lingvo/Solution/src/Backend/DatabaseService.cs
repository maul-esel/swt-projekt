using System;

using Microsoft.Extensions.Configuration;

using Lingvo.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Server.Kestrel;
using System.Collections.Generic;
using System.Linq;

namespace Lingvo.Backend
{
	using Common.Entities;

	public class DatabaseService : DbContext
    {
		private static DbContextOptions<DatabaseService> options;

		public DbSet<Workbook> Workbooks { get; set; }
		public DbSet<Page> Pages { get; set; }
		public DbSet<Recording> Recordings { get; set; }

		public DatabaseService(DbContextOptions<DatabaseService> options) : base(options)
    	{ }

		public static void Connect(string connectionString)
		{
			options = new DbContextOptionsBuilder<DatabaseService>()
				.UseMySql(connectionString)
				.Options;
		}

		public static DatabaseService getNewContext()
		{
			return new DatabaseService(options);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Workbook>().Property(w => w.Title).IsRequired();
			modelBuilder.Entity<Workbook>().Property(w => w.Subtitle).IsRequired();
			modelBuilder.Entity<Workbook>().Property(w => w.LastModified).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP").HasComputedColumnSql("CURRENT_TIMESTAMP");
			modelBuilder.Entity<Workbook>().HasMany(w => (IEnumerable<Page>) w.Pages).WithOne(p => p.Workbook).HasForeignKey(p => p.workbookId);

			modelBuilder.Entity<Page>().Property(p => p.Description).IsRequired();
			modelBuilder.Entity<Page>().HasOne(p => p.StudentTrack).WithMany().HasForeignKey(p => p.studentTrackId);
			modelBuilder.Entity<Page>().HasOne(p => p.TeacherTrack).WithMany().HasForeignKey(p => p.teacherTrackId);

			modelBuilder.Entity<Recording>().Property(r => r.LocalPath).IsRequired();
		}

		public List<Workbook> GetWorkbooksWithReferences()
		{
			return Workbooks.Include(w => w.Pages).ThenInclude(p => p.TeacherTrack).ToList();
		}

		public List<Page> GetPagesWithReferences()
		{
			return Pages.Include(p => p.TeacherTrack).ToList();
		}

		public Workbook FindWorkbookWithReferences(int id)
		{
			var workbook = Find<Workbook>(id);
			Entry(workbook).Collection(w => (IEnumerable<Page>) w.Pages)
				.Query()
				.Include(p => p.TeacherTrack)
				.Load();
			return workbook;
		}

		/// <summary>
		/// Save the specified recording, updates it if it already exists.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="recording">Recording.</param>
		public void Save(Recording recording)
		{
			if (Recordings.Find(recording.Id) != null)
			{
				Recordings.Remove(Recordings.Find(recording.Id));
			}
			Recordings.Add(recording);
			SaveChanges();
		}

		/// <summary>
		/// Save the specified page, updates it if it already exists.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="page">Page.</param>
		public void Save(Page page)
		{
			if (Pages.Find(page.Id) != null)
			{
				Recordings.Remove(Recordings.Find(page.Id));
			}
			Pages.Add(page);
			SaveChanges();
		}

		/// <summary>
		/// Save the specified workbook, updates it if it already exists.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="workbook">Workbook.</param>
		public void Save(Workbook workbook)
		{
			if (Workbooks.Find(workbook.Id) != null)
			{
				Recordings.Remove(Recordings.Find(workbook.Id));
			}
			Workbooks.Add(workbook);
			SaveChanges();
		}


    }
}
