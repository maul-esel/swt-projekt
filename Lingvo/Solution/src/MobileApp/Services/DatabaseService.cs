using LinqToDB.DataProvider.SQLite;

namespace Lingvo.MobileApp.Services
{
	public class DatabaseService : Common.Services.AbstractDatabaseService
	{
		public DatabaseService(string connectionString)
			: base(new SQLiteDataProvider(), connectionString) { }
	}
}
