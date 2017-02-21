using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

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
			//Navigate to Download tap
			app.Tap(c => c.Text("Download"));
			//click on one workbook
			app.Tap(c => c.Marked("Thannhauser Modell"));

			//download 1 page
			app.Repl();
		}
	}
}
