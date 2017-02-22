using NUnit.Framework;
using Xamarin.UITest;
using System.Threading;
using Xamarin.UITest.Queries;
using System.Collections.Generic;

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

            //Wait until page is rendered
            Thread.Sleep(500);

            //Look for Play/Pause Button
            var ppbutton = app.Query(c => c.Marked("PlayPauseButton"));

            //Assert it exists
            Assert.AreEqual(ppbutton.Length, 1);
        }

        [Test]
        public void EditPageTest()
        {
            int timeout = 2000;
            int tolerance = 500;

            //Open the page
            SelectPageTest();

            //Start recording
            app.Tap(c => c.Marked("RecordStopButton"));

            //Let the recorder record for a while
            Thread.Sleep(timeout);

            //Stop recording
            app.Tap(c => c.Marked("RecordStopButton"));

            //Give it time to save
            Thread.Sleep(tolerance);

            //Check if the mute button is visible (=> StudentTrack exists)
            Assert.Greater(app.Query(c => c.Marked("MuteButton")).Length, 0);
        }

        [Test]
        public void PlayPageTest()
        {
            int timeout = 2000;
            int tolerance = 500;

            SelectPageTest();

            //Play the page
            app.Tap(c => c.Marked("PlayPauseButton"));

            Thread.Sleep(timeout - tolerance);

            //Pause playing
            app.Tap(c => c.Marked("PlayPauseButton"));

            //app.Repl();

            //Check if the progress is at 00:02
            var progressQuery = app.Query(c => c.Marked("00:02"));

            Assert.Greater(progressQuery.Length, 0);

            //Resume playing
            app.Tap(c => c.Marked("PlayPauseButton"));

            Thread.Sleep(timeout - tolerance);

            //Pause again
            app.Tap(c => c.Marked("PlayPauseButton"));

            //Check if the progress is between 00:03 and 00:05 (Xamarin UITest is slow sometimes)
            var progressList = new List<AppResult>();
            progressList.AddRange(app.Query(c => c.Marked("00:03")));
            progressList.AddRange(app.Query(c => c.Marked("00:04")));
            progressList.AddRange(app.Query(c => c.Marked("00:05")));

            Assert.Greater(progressQuery.Length, 0);

            app.Tap(c => c.Marked("RewindButton"));

            //Check if the progress is at 00:00
            progressQuery = app.Query(c => c.Marked("00:00"));

            Assert.Greater(progressQuery.Length, 0);

            app.Tap(c => c.Marked("ForwardButton"));

            //Check if the progress is at 00:05
            progressQuery = app.Query(c => c.Marked("00:05"));

            Assert.Greater(progressQuery.Length, 0);

            //Stop the playing and start a recording

            app.Tap(c => c.Marked("RecordStopButton"));
            app.Tap(c => c.Marked("RecordStopButton"));

            //Let the recorder record for a while
            Thread.Sleep(timeout);

            //Stop recording
            app.Tap(c => c.Marked("RecordStopButton"));

            //Give it time to save
            Thread.Sleep(tolerance);

            app.Tap(c => c.Marked("MuteButton"));

            //Check if student Track is muted (means progressView is aware of it)
            var isMutedQuery = app.Query(c => c.Marked("StudentMuted"));

            Assert.Greater(isMutedQuery.Length, 0);
        }

        [Test]
        public void DeleteStudentTrackTest()
        {
            int timeout = 500;

            //Create a student recording
            EditPageTest();

            app.Back();

            //Open context menu for page
            app.TouchAndHoldCoordinates(692, 262);

            //Select delete student track
            app.Tap(c => c.Marked("Aufnahme löschen"));

            //Click ok on warning dialog
            app.TapCoordinates(570, 730);

            Thread.Sleep(timeout);

            //Navigate into audio page
            app.TapCoordinates(692, 262);

            //Check if the mute button is invisible (=> StudentTrack does not exists)
            Assert.AreEqual(app.Query(c => c.Marked("MuteButton")).Length, 0);
        }

        [Test]
        public void DeletePageTest()
        {
            int timeout = 500;

            //Open workbook
            app.Tap(c => c.Marked("Thannhauser Modell"));

            //Open context menu for page
            app.TouchAndHoldCoordinates(692, 262);

            //Select delete
            app.Tap(c => c.Marked("Löschen"));

            //Click ok on warning dialog
            app.TapCoordinates(570, 730);

            Thread.Sleep(500);

            Assert.Greater(app.Query(c => c.Text("Arbeitshefte")).Length, 0);

            //Count downloaded workbooks
            var numWorkbooks = app.Query(q => q.Class("ListView").Child()).Length;

            //No workbook should exist because the only page was deleted
            Assert.AreEqual(numWorkbooks, 0);
        }
    }
}
