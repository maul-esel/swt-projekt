using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace Lingvo.Backend.Services
{
	using Common.Entities;

	/// <summary>
	/// Encapsulates access to the database used for storing
	/// <see cref="Workbook"/>s, <see cref="Page"/>s, <see cref="Rec"/>s
	/// and <see cref="Editor"/>s.
	/// </summary>
	public class DatabaseService : DbContext
    {
		/// <summary>
		/// Exposes the list of <see cref="Workbook"/>s stored in the database.
		/// </summary>
		public DbSet<Workbook> Workbooks { get; set; }

		/// <summary>
		/// Exposes the list of <see cref="Page"/>s stored in the database.
		/// </summary>
		public DbSet<Page> Pages { get; set; }

		/// <summary>
		/// Exposes the list of <see cref="Recording"/>s stored in the database.
		/// </summary>
		public DbSet<Recording> Recordings { get; set; }

		/// <summary>
		/// Exposes the list of <see cref="Editor"/>s stored in the database.
		/// </summary>
		public DbSet<Editor> Editors { get; set; }

		/// <summary>
		/// Creates a new instance. The <paramref name="options"/> are configured
		/// in <see cref="Startup.ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection)"/>
		/// and supplied by dependency injection.
		/// </summary>
		public DatabaseService(DbContextOptions<DatabaseService> options) : base(options)
		{
		}

		/// <summary>
		/// Manually creates an instance of the class with the given <paramref name="connectionString"/>.
		/// Typically, instances are not created manually but supplied by dependency injection.
		/// </summary>
		/// <param name="connectionString"></param>
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

			modelBuilder.Entity<Page>().HasOne(p => p.StudentTrack).WithMany().HasForeignKey(p => p.studentTrackId);
			modelBuilder.Entity<Page>().HasOne(p => p.TeacherTrack).WithMany().HasForeignKey(p => p.teacherTrackId);

			modelBuilder.Entity<Recording>().Property(r => r.LocalPath).IsRequired();
			modelBuilder.Entity<Recording>().Property(r => r.CreationTime).HasDefaultValueSql("CURRENT_TIMESTAMP");

			modelBuilder.Entity<Editor>().Property(e => e.Name).IsRequired();
			modelBuilder.Entity<Editor>().Property(e => e.PasswordHash).IsRequired();
			modelBuilder.Entity<Editor>().HasKey(e => e.Name);
		}

		/// <summary>
		/// Loads all workbooks, their pages and the associated recordings from the database.
		/// </summary>
		public List<Workbook> GetWorkbooksWithReferences()
		{
			return Workbooks.Include(w => w.Pages).ThenInclude(p => p.TeacherTrack).ToList();
		}

		/// <summary>
		/// Loads all pages with their associated recordings from the database.
		/// </summary>
		public List<Page> GetPagesWithReferences()
		{
			return Pages.Include(p => p.TeacherTrack).ToList();
		}

		/// <summary>
		/// Finds the workbook with the given <paramref name="id"/> and loads it from
		/// the database, including its pages and their associated recordings.
		/// </summary>
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

		/// <summary>
		/// Finds the page with the given <paramref name="id"/>
		/// and loads it from the database, including the associated <see cref="Page.TeacherTrack"/>.
		/// </summary>
		public Page FindPageWithRecording(int id)
		{
			var page = Pages.Find(id);
			if (page == null)
				return null;

			Entry(page).Reference(p => p.TeacherTrack).Load();
			return page;
		}

		/// <summary>
		/// Saves the specified <paramref name="recording"/>, updates it if it already exists.
		/// </summary>
		public void Save(Recording recording)
		{
			if (Recordings.Find(recording.Id) == null)
			{
				Recordings.Add(recording);
			}
			SaveChanges();
		}

		/// <summary>
		/// Saves the specified <paramref name="page"/>, updates it if it already exists.
		/// </summary>
		public void Save(Page page)
		{
			if (Pages.Find(page.Id) == null)
			{
				Pages.Add(page);
			}
			SaveChanges();
		}

		/// <summary>
		/// Saves the specified <paramref name="workbook"/>, updates it if it already exists.
		/// </summary>
		public void Save(Workbook workbook)
		{
			if (Workbooks.Find(workbook.Id) == null)
			{
				Workbooks.Add(workbook);
			}
			SaveChanges();
		}

		/// <summary>
		/// Deletes the specified <paramref name="recording"/>.
		/// </summary>
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
		/// Deletes the specified <paramref name="page"/> and the associated recording.
		/// </summary>
		public void Delete(Page page)
		{
			var p = Pages.Find(page.Id);

			if (p == null)
			{
				return;
			}

			var r = Recordings.Find(p.teacherTrackId);
			if (r != null)
			{
				Recordings.Remove(r);
			}

			Pages.Remove(page);
			SaveChanges();
		}

		/// <summary>
		/// Delete the specified <paramref name="workbook"/> and all corresponding pages.
		/// </summary>
		public void Delete(Workbook workbook)
		{
			// load workbook with references in order to delete their pages
			var w = FindWorkbookWithReferences(workbook.Id);

			if (w != null)
			{
				// pages must be deleted explicitly in order to also delete recordings and their files
				foreach (var page in w.Pages.ToList())
					Delete((Page)page);

				Workbooks.Remove(w); 
				SaveChanges();
			}
		}
	}
}
