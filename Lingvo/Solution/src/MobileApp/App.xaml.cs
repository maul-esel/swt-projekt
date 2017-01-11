using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using LinqToDB.DataProvider.SQLite;

namespace Lingvo.MobileApp
{
	using Common.Services;

	public partial class App : Application
	{
		const string databasePath = "db.sqlite";
		public static DatabaseService Database { get; private set; }

		private static void SetupDatabaseConnection()
		{
			if (Database == null)
				Database = DatabaseService.Connect<SQLiteDataProvider>($"Data Source=${databasePath}");
		}

		public App ()
		{
			SetupDatabaseConnection();
			InitializeComponent();

			MainPage = new MobileApp.MainPage();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
