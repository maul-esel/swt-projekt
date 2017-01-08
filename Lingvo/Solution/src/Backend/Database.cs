using System;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.MySql;

namespace Lingvo.Backend
{
	using Common.Entities;

    public static class Database
    {
		internal static string ConnectionString { get; set; }

		private static DataConnection _connection;

		private static DataConnection Connection
		{
			get
			{
				if (_connection == null)
				{
					_connection = MySqlTools.CreateDataConnection(ConnectionString);
					_connection.MappingSchema.SetConverter<int, TimeSpan>(ms => TimeSpan.FromMilliseconds(ms));
				}
				return _connection;
			}
		}

		public static ITable<Recording> Recordings => Connection.GetTable<Recording>();
		public static ITable<Page> Pages => Connection.GetTable<Page>()
			.LoadWith(p => p.Workbook)
			.LoadWith(p => p.TeacherTrack);
		public static ITable<Workbook> Workbooks => Connection.GetTable<Workbook>()
			.LoadWith(w => w.Pages);

		public static Page Find(this ITable<Page> pages, int workbookId, int pageNumber)
		{
			return pages.FirstOrDefault(page => page.workbookId == workbookId && page.Number == pageNumber);
		}
    }
}
