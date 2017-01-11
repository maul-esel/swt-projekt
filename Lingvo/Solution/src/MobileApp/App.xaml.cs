using System;
using System.IO;
using System.Reflection;

using Xamarin.Forms;
using Microsoft.Data.Sqlite;

namespace Lingvo.MobileApp
{
	using Common.Entities;
	using Services;

	public partial class App : Application
	{
		const string databasePath = "lingvo.sqlite";

#if __ANDROID__
		const string sqlResource = "Lingvo.MobileApp.Droid.SQL.client.sql";
#elif __IOS__
		const string sqlResource = "Lingvo.MobileApp.iOS.SQL.client.sql";
#endif
		public static DatabaseService Database { get; private set; }

		private static void SetupDatabaseConnection()
		{
			if (Database == null)
			{
				SqliteConnectionStringBuilder b = new SqliteConnectionStringBuilder() { };
				b.DataSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), databasePath);
				Database = new DatabaseService(b.ToString());
				Database.Execute(ReadDatabaseDefinition());
			}
		}

		private static string ReadDatabaseDefinition()
		{
			var assembly = typeof(App).GetTypeInfo().Assembly;
			var stream = assembly.GetManifestResourceStream(sqlResource);
			using (var reader = new StreamReader(stream))
				return reader.ReadToEnd();
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
