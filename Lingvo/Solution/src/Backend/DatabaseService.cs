using System;

using LinqToDB.Mapping;
using LinqToDB.DataProvider.MySql;
using LinqToDB;

using Microsoft.Extensions.Configuration;

using Lingvo.Common.Services;

namespace Lingvo.Backend
{
	using Common.Entities;

    public class DatabaseService : Common.Services.DatabaseService
    {
		public DatabaseService(string connectionString)
			: base(new MySqlDataProvider(), connectionString)
		{
			AdjustMappingSchema(connection.MappingSchema);
		}

		public static DatabaseService Connect(IConfiguration config)
		{
			return new DatabaseService(
				$"Server={config["DB_HOST"]};Port={config["DB_PORT"]};Database={config["DB_NAME"]};Uid={config["DB_USER"]};Pwd={config["DB_PASSWORD"]};charset=utf8;"
			);
		}

		private void AdjustMappingSchema(MappingSchema schema)
		{
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
					.HasSkipOnUpdate(true);
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
				int id = Convert.ToInt32((UInt64)connection.InsertWithIdentity(recording));
				recording.Id = id;
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
				int id = Convert.ToInt32((UInt64) connection.InsertWithIdentity(workbook));
				workbook.Id = id;
			}
		}
    }
}
