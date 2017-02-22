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
	public class RemoteTests
	{
		IApp app;
		Platform platform;

		public RemoteTests(Platform platform)
		{
			this.platform = platform;
		}

		[SetUp]
		public void BeforeEachTest()
		{
			app = AppInitializer.StartApp(platform);
		}

		[Test]
		public void DownloadWorkbookTest()
		{
			//Navigate to Download tap
			app.Tap(c => c.Text("Download"));

            //Wait for server sync
            Thread.Sleep(3000);

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

            //Count downloaded workbooks
			var numWorkbooks = app.Query(q => q.Class("ListView").Child()).Length;

			Thread.Sleep(1000);

			//click on diktate und mehr
			app.Tap(c => c.Marked("Diktate und mehr"));

            //Get all visible pages
			var pageTexts = new List<AppResult>(app.Query(q => q.Class("FormsTextView")));

			app.ScrollDown();

            //... and the rest of the pages
			pageTexts.AddRange(app.Query(q => q.Class("FormsTextView")));

            //Eliminate Page-Descriptions
			pageTexts.RemoveAll(p => !p.Text.Contains("Seite"));

            //Count wrong page numbers to determine correctness
			bool containsAllPages = null == pageTexts.Find(p =>
			{
				int number = Int32.Parse(p.Text.Replace("Seite ", ""));
				return number < 0 || number > 9;
			});

            //Two workbooks and all pages of the second one should be downloaded
			Assert.IsTrue(numWorkbooks == 2 && 
			             containsAllPages);
		}
	}
}
