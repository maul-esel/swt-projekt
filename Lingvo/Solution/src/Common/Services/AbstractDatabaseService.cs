using System;
using System.Linq;
using LinqToDB;
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
				if (sqlType == LinqToDB.SqlQuery.SqlDataType.Int32)
					sql.Append(((TimeSpan)value).Milliseconds);
				throw new NotSupportedException();
			});
		}

		public ITable<Recording> Recordings => connection.GetTable<Recording>();

		public ITable<Page> Pages => connection.GetTable<Page>()
			.LoadWith(p => p.Workbook)
			.LoadWith(p => p.TeacherTrack);

		public ITable<Workbook> Workbooks => connection.GetTable<Workbook>()
			// .LoadWith(w => w.Pages) // TODO: association type currently unsupported
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
