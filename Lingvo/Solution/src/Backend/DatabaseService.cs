using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using System;

namespace Lingvo.Backend
{
	using Common.Entities;

	public class DatabaseService : DbContext
    {
		public DbSet<Workbook> Workbooks { get; set; }
		public DbSet<Page> Pages { get; set; }
		public DbSet<Recording> Recordings { get; set; }
		public DbSet<Editor> Editors { get; set; }

		public DatabaseService(DbContextOptions<DatabaseService> options) : base(options)
    	{ }

		public static DatabaseService Connect(string connectionString)
		{
			return new DatabaseService(
				new DbContextOptionsBuilder<DatabaseService>()
					.UseMySql(connectionString)
					.Options
			);
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
			modelBuilder.Entity<Recording>().Property(r => r.CreationTime).HasDefaultValueSql("CURRENT_TIMESTAMP");

			modelBuilder.Entity<Editor>().Property(e => e.Name).IsRequired();
			modelBuilder.Entity<Editor>().Property(e => e.PasswordHash).IsRequired();
			modelBuilder.Entity<Editor>().HasKey(e => e.Name);
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
			if (workbook == null)
				return null;

			Entry(workbook).Collection(w => (IEnumerable<Page>) w.Pages)
				.Query()
				.Include(p => p.TeacherTrack)
				.Load();
			return workbook;
		}

		public Page FindPageWithRecording(int id)
		{
			var page = Find<Page>(id);
			if (page == null)
				return null;

			Entry(page).Reference(p => p.TeacherTrack).Load();
			return page;
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
				var r = Recordings.Find(recording.Id);
				r.Duration = recording.Duration;
				r.LocalPath = recording.LocalPath;
			}
			else
			{
				Recordings.Add(recording);
			}
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
				var p = Pages.Find(page.Id);
				p.Description = page.Description;
				p.Number = page.Number;
				p.StudentTrack = page.StudentTrack;
				p.studentTrackId = page.studentTrackId;
				p.teacherTrackId = page.teacherTrackId;
				p.TeacherTrack = page.TeacherTrack;
				p.workbookId = page.workbookId;
				p.Workbook = page.Workbook;
			}
			else
			{
				Pages.Add(page);
			}
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
				var w = Workbooks.Find(workbook.Id);
				w.Title = workbook.Title;
				w.Subtitle = workbook.Subtitle;
				w.Pages = workbook.Pages;
				w.TotalPages = workbook.TotalPages;
			}
			else
			{
				Workbooks.Add(workbook);
			}
			SaveChanges();
		}

		/// <summary>
		/// Delete the specified recording.
		/// </summary>
		/// <returns>The delete.</returns>
		/// <param name="recording">Recording.</param>
		public void Delete(Recording recording)
		{
			var r = Recordings.Find(recording.Id);
			if (r != null)
			{
				Recordings.Remove(r);
				SaveChanges();
			}
		}

		/// <summary>
		/// Delete the specified page and the recording belonging to it.
		/// </summary>
		/// <returns>The delete.</returns>
		/// <param name="page">Page.</param>
		public void Delete(Page page)
		{
			var p = Pages.Find(page.Id);

			if (p == null)
			{
				return;
			}

			var r = Recordings.Find(page.teacherTrackId);
			if (r != null)
			{
				Recordings.Remove(r);
			}

			Pages.Remove(p);
			SaveChanges();
		}

		/// <summary>
		/// Delete the specified workbook and all corresponding pages.
		/// </summary>
		/// <returns>The delete.</returns>
		/// <param name="workbook">Workbook.</param>
		public void Delete(Workbook workbook)
		{
			var w = Workbooks.Find(workbook.Id);

			if (w != null)
			{
				foreach (var p in workbook.Pages)
				{
					Delete((Page) p);
				}

				Workbooks.Remove(w); 
				SaveChanges();
			}
		}
    }
}
