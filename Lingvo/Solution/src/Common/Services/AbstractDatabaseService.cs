using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Mapping;
using LinqToDB.Data;
using LinqToDB.DataProvider;

namespace Lingvo.Common.Services
{
	using Entities;

    public abstract class AbstractDatabaseService
    {
		protected readonly DataConnection connection;

		static AbstractDatabaseService() {
			LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
		}

		public AbstractDatabaseService(IDataProvider provider, string connectionString)
		{
			connection = new DataConnection(provider, connectionString);
			connection.MappingSchema.SetConverter<int, TimeSpan>(ms => TimeSpan.FromMilliseconds(ms));
			connection.MappingSchema.SetValueToSqlConverter(typeof(TimeSpan), (sql, sqlType, value) =>
			{
				//if (sqlType == LinqToDB.SqlQuery.SqlDataType.Int32)
					sql.Append(((TimeSpan)value).Milliseconds);
				Console.WriteLine(sql.ToString());
			});
			BuildMappingSchema(connection.MappingSchema);
		}

		protected virtual void BuildMappingSchema(MappingSchema schema)
		{
			schema.GetFluentMappingBuilder()
			.Entity<Recording>()
				.HasTableName("Recordings")
				.HasColumn(r => r.Id)
				.HasColumn(r => r.CreationTime)
				.HasColumn(r => r.Length)
				.HasColumn(r => r.LocalPath)

				.Property(r => r.Id)
					.IsPrimaryKey()
					.IsNullable(false)

				.Property(r => r.CreationTime)
					.IsNullable(false)

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
					.IsNullable(false)
				.Property(w => w.Title).IsNullable(false)
				.Property(w => w.Subtitle).IsNullable(false)
				.Property(w => w.IsPublished)
					.IsNullable(false)
					.HasColumnName("published")
				.Property(w => w.LastModified).IsNullable(false)
				.Property(w => w.TotalPages).IsNullable(false)

				.Property(w => w.Pages)
					.HasAttribute(new AssociationAttribute() { ThisKey = nameof(Workbook.Id), OtherKey = nameof(Page.workbookId) });
		}

		public ITable<Recording> Recordings => connection.GetTable<Recording>();

		public ITable<Page> Pages => connection.GetTable<Page>()
			.LoadWith(p => p.Workbook)
			.LoadWith(p => p.TeacherTrack);

		public ITable<Workbook> Workbooks => connection.GetTable<Workbook>()
			//.LoadWith(w => w.Pages) // TODO: association type currently unsupported
			;

		public void Execute(string sql)
		{
			connection.Execute(sql);
		}

	}

	public static class DatabaseExtensions
	{
		public static Page Find(this ITable<Page> pages, int workbookId, int pageNumber)
		{
			return pages.FirstOrDefault(page => page.workbookId == workbookId && page.Number == pageNumber);
		}

		public static Workbook Find(this ITable<Workbook> workbooks, int workbookId)
		{
			return workbooks.FirstOrDefault(workbook => workbook.Id == workbookId);
		}

		public static Recording Find(this ITable<Recording> recordings, int recordingId)
		{
			return recordings.FirstOrDefault(recording => recording.Id == recordingId);
		}
	}
}
