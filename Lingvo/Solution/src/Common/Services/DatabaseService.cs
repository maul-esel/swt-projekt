using System;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;

namespace Lingvo.Common.Services
{
	using Entities;

    public class DatabaseService
    {
		private readonly DataConnection connection;

		static DatabaseService() {
			LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
		}

		public static DatabaseService Connect<T>(string connectionString) where T : IDataProvider, new()
		{
			return new DatabaseService(new T(), connectionString);
		}

		public DatabaseService(IDataProvider provider, string connectionString)
		{
			connection = new DataConnection(provider, connectionString);
			connection.MappingSchema.SetConverter<int, TimeSpan>(ms => TimeSpan.FromMilliseconds(ms));
			// TODO: converter from value to sql
		}

		public ITable<Recording> Recordings => connection.GetTable<Recording>();

		public ITable<Page> Pages => connection.GetTable<Page>()
			.LoadWith(p => p.Workbook)
			.LoadWith(p => p.TeacherTrack);

		public ITable<Workbook> Workbooks => connection.GetTable<Workbook>()
			//.LoadWith(w => w.Pages) // TODO: association type currently unsupported
			;
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
