using LinqToDB.DataProvider.SQLite;
using LinqToDB;
using Lingvo.Common.Entities;
using Lingvo.Common.Services;

namespace Lingvo.MobileApp.Services
{
	public class DatabaseService : Common.Services.DatabaseService
	{
		public DatabaseService(string connectionString)
			: base(new SQLiteDataProvider(), connectionString) { }
	
	
		/// <summary>
		/// Save the specified recording, updates it if it already exists.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="recording">Recording.</param>
		public void Save(Recording recording)
		{

			connection.InsertOrReplace(recording);

		}

		/// <summary>
		/// Save the specified page, updates it if it already exists.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="page">Page.</param>
		public void Save(Page page)
		{
			
			connection.InsertOrReplace(page);

		}

		/// <summary>
		/// Save the specified workbook, updates it if it already exists.
		/// </summary>
		/// <returns>The save.</returns>
		/// <param name="workbook">Workbook.</param>
		public void Save(Workbook workbook)
		{

			connection.InsertOrReplace(workbook);

		}
	}
}
