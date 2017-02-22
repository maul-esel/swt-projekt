using System;
using System.IO;
using Xamarin.Forms;

namespace Lingvo.MobileApp
{
	using Services;

	/// <summary>
	/// The core application class for the entire app.
	/// </summary>
	public partial class App : Application
	{
		const string databasePath = "lingvo.sqlite";

		/// <summary>
		/// The <see cref="DatabaseService"/> instance used to access the SQLite database.
		/// </summary>
		public static DatabaseService Database { get; private set; }

		private static void SetupDatabaseConnection()
		{
			if (Database == null)
			{
				string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), databasePath);
				Database = new DatabaseService(dbPath);
				Database.createTables();
			}
		}

		public App()
		{
			SetupDatabaseConnection();
			InitializeComponent();

			MainPage = new MobileApp.Pages.MainPage();
		}
	}
}
