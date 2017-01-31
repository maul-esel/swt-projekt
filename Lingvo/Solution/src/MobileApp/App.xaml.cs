using System;
using System.IO;
using System.Reflection;

using Xamarin.Forms;


namespace Lingvo.MobileApp
{
	using Common.Entities;
	using Services;

	public partial class App : Application
	{
		const string databasePath = "lingvo.sqlite";

		public static LingvoMobileContext Database { get; private set; }

		private static void SetupDatabaseConnection()
		{
			if (Database == null)
			{
				string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), databasePath);
				Database = new LingvoMobileContext(dbPath);
				Database.createTables();
			}
		}

		public App ()
		{
			SetupDatabaseConnection();
			InitializeComponent();

			MainPage = new MobileApp.Pages.MainPage();
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
