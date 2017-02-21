using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using System.Collections.Generic;
using Xamarin.UITest.Queries;
using System.Threading;

namespace MobileApp.Droid.UITests
{
	[TestFixture(Platform.Android)]
	public class Tests
	{
		IApp app;
		Platform platform;

		public Tests(Platform platform)
		{
			this.platform = platform;
		}

		[SetUp]
		public void BeforeEachTest()
		{
			app = AppInitializer.StartApp(platform);
		}

		[Test]
		public void AppLaunches()
		{
			//app.Repl();
		}

		[Test]
		public void DownloadWorkbookTest()
		{
			app.Repl();

			//Navigate to Download tap
			app.Tap(c => c.Text("Download"));

			//click on one workbook
			app.Tap(c => c.Marked("Thannhauser Modell"));

			//click on one page
			app.TapCoordinates(692, 262);

			//download with mobile data allowed (emulator never has wifi)
			app.TapCoordinates(570, 730);

			Thread.Sleep(5000);

			app.Back();

			//download whole workbook
			app.TapCoordinates(710, 580);

			//download with mobile data allowed (emulator never has wifi)
			app.TapCoordinates(570, 730);

			Thread.Sleep(20000);

			//Navigate to Download tap
			app.Tap(c => c.Text("Arbeitshefte"));

			var numWorkbooks = app.Query(q => q.Class("ListView").Child()).Length;

			Thread.Sleep(1000);

			//click on diktate und mehr
			app.Tap(c => c.Marked("Diktate und mehr"));

			//var visiblePageViews = app.Query(q => q.Class("ListView").Child());
			var pageTexts = new List<AppResult>(app.Query(q => q.Class("FormsTextView")));

			app.ScrollDown();

			pageTexts.AddRange(app.Query(q => q.Class("FormsTextView")));

			pageTexts.RemoveAll(p => !p.Text.Contains("Seite"));

			bool containsAllPages = null == pageTexts.Find(p =>
			{
				int number = Int32.Parse(p.Text.Replace("Seite ", ""));
				return number < 0 || number > 9;
			});

			Assert.IsTrue(numWorkbooks == 2 && 
			             containsAllPages);

		}
	}
}
