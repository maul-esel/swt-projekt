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
    public class LocalTests
    {
        IApp app;
        Platform platform;

        public LocalTests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);

            /*--- Download a page to work with ---*/

            //Navigate to Download tap
            app.Tap(c => c.Text("Download"));

            //click on one workbook
            app.Tap(c => c.Marked("Thannhauser Modell"));

            //click on one page
            app.TapCoordinates(692, 262);

            //download with mobile data allowed (emulator never has wifi)
            app.TapCoordinates(570, 730);

            Thread.Sleep(5000);

            //Go back to LocalCollectionPage
            app.Back();
            app.Tap(c => c.Text("Arbeitshefte"));
        }

        [Test]
        public void SelectPageTest()
        {
            //Open workbook
            app.Tap(c => c.Marked("Thannhauser Modell"));

            //Open page
            app.TapCoordinates(692, 262);

            //Look for Play/Pause Button
            var ppbutton = app.Query(c => c.Marked("PlayPauseButton"));

            //Assert it exists
            Assert.AreEqual(ppbutton.Length, 1);
        }
    }
}
