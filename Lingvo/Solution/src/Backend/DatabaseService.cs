using System.Collections.Generic;
using LinqToDB.Mapping;
using LinqToDB.DataProvider.MySql;
using LinqToDB;
using Lingvo.Common.Services;

namespace Lingvo.Backend
{
	using Common.Entities;

    public class DatabaseService : Common.Services.AbstractDatabaseService
    {
		public DatabaseService(string connectionString)
			: base(new MySqlDataProvider(), connectionString) { }

		protected override void BuildMappingSchema(MappingSchema schema)
		{
			schema.GetFluentMappingBuilder()
			.Entity<Recording>()
				.HasTableName("Recordings")
				.HasColumn(r => r.Id)
				.HasColumn(r => r.CreationTime)
				.HasColumn(r => r.Length)
				.HasColumn(r => r.LocalPath)

				.Property(r => r.Id).IsIdentity()
					.IsPrimaryKey()
					.IsNullable(false)

				.Property(r => r.CreationTime)
					.IsNullable(false)
						.HasSkipOnInsert(true)
						.HasSkipOnUpdate(true)

				.Property(r => r.Length).IsNullable(false)
				.Property(r => r.LocalPath).IsNullable(false)

			.Entity<Page>()
				.HasTableName("Pages")
				.HasColumn(p => p.teacherTrackId)
				.HasColumn(p => p.studentTackId)
				.HasColumn(p => p.workbookId)
				.HasColumn(p => p.Number)
				.HasColumn(p => p.Description)

				.Property(p => p.teacherTrackId).IsNullable(false)
				.Property(p => p.studentTackId).IsNullable(false)
				.Property(p => p.workbookId).IsNullable(false)
				.Property(p => p.Number).IsNullable(false)
				.Property(p => p.Description).IsNullable(false)

				.Association(p => p.Workbook, p => p.workbookId, w => w.Id)
					.IsNullable(false)
				.Association(p => p.TeacherTrack, p => p.teacherTrackId, r => r.Id)
					.IsNullable(false)
				.Association(p => p.StudentTrack, p => p.studentTackId, r => r.Id)
					.IsNullable(false)

			.Entity<Workbook>()
				.HasTableName("Workbooks")
				.HasColumn(w => w.Id)
				.HasColumn(w => w.Title)
				.HasColumn(w => w.Subtitle)
				.HasColumn(w => w.IsPublished)
				.HasColumn(w => w.LastModified)
				.HasColumn(w => w.TotalPages)

				.Property(w => w.Id)
					.IsPrimaryKey()
					.IsIdentity()
					.IsNullable(false)
				.Property(w => w.Title).IsNullable(false)
				.Property(w => w.Subtitle).IsNullable(false)
				.Property(w => w.IsPublished)
					.IsNullable(false)
					.HasColumnName("published")
				.Property(w => w.LastModified).IsNullable(false)
						.HasSkipOnInsert(true)
						.HasSkipOnUpdate(true)
				.Property(w => w.TotalPages).IsNullable(false)

				.Property(w => w.Pages)
					.HasAttribute(new AssociationAttribute() { ThisKey = nameof(Workbook.Id), OtherKey = nameof(Page.workbookId) });
/*			base.BuildMappingSchema(schema);
			schema.GetFluentMappingBuilder()
				.Entity<Recording>()
					.Property(r => r.Id).IsIdentity()
					.Property(r => r.CreationTime)
						.HasSkipOnInsert(true)
						.HasSkipOnUpdate(true)
				.Entity<Workbook>()
					.Property(w => w.Id).IsIdentity()
					.Property(w => w.LastModified)
						.HasSkipOnInsert(true)
						.HasSkipOnUpdate(true);*/
		}

		/// <summary>
		/// Save the specified recording, updates it if it already exists.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="recording">Recording.</param>
		public void Save(Recording recording)
		{
			if (Recordings.Find(recording.Id) != null) {
				connection.Update(recording);
			}
			else
			{
				connection.InsertWithIdentity(recording);
			}
		}

		/// <summary>
		/// Save the specified page, updates it if it already exists.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="page">Page.</param>
		public void Save(Page page)
		{
			if (Pages.Find(page.workbookId, page.Number) != null)
			{
				connection.Update(page);
			}
			else
			{
				connection.InsertWithIdentity(page);
			}
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
				connection.Update(workbook);
			}
			else
			{
				connection.InsertWithIdentity(workbook);
			}
		}
    }
}
