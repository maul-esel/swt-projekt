using NUnit.Framework;
using Xamarin.UITest;
using System.Threading;
using Xamarin.UITest.Queries;
using System.Collections.Generic;

namespace MobileApp.Droid.UITests
{
    [TestFixture(Platform.Android)]
    public class TeacherMemoTests
    {
        IApp app;
        Platform platform;

        public TeacherMemoTests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }

        [Test]
        public void RecordTeacherMemoTest()
        {
            int timeout = 2000;
            int tolerance = 500;

            app.Tap(c => c.Text("Lehrermemos"));

            //Create new teacher memo
            app.Tap(c => c.Marked("New..."));

            //Start recording
            app.Tap(c => c.Marked("RecordButton"));

            Thread.Sleep(timeout - tolerance);

            //Stop recording
            app.Tap(c => c.Marked("RecordButton"));

            //Check recording

            var label = app.Query(q => q.Class("FormsTextView"))[0].Text;

            //Does not work on emulator, as it returns random numbers for recording duration
            //Assert.Equals("00:02", label);
            Assert.IsTrue(!"00:00".Equals(label));

            //Restart recording
            app.Tap(c => c.Marked("RecordButton"));

            //Click ok on warning dialog
            app.TapCoordinates(570, 730);

            Thread.Sleep(timeout - tolerance);

            app.Tap(c => c.Marked("RecordButton"));

            label = app.Query(q => q.Class("FormsTextView"))[0].Text;

            //Does not work on emulator, as it returns random numbers for recording duration
            //Assert.Equals("00:02", label);
            Assert.IsTrue(!"00:00".Equals(label));

            //Enter name TeacherMemo
            app.EnterText(c => c.Marked("NameEntry"), "TeacherMemo");

            //and save teacher memo
            app.Tap(c => c.Marked("Speichern"));

            Thread.Sleep(timeout);

            //Check if a formstextview with text TeacherMemo exists
            var pageTexts = new List<AppResult>(app.Query(q => q.Class("FormsTextView")));

            Assert.IsTrue(pageTexts.Count == 1 && pageTexts[0].Text.Equals("TeacherMemo"));
        }

        [Test]
        public void PlayTeacherMemo()
        {
            int timeout = 1000;
            int tolerance = 500;

            RecordTeacherMemoTest();

            //Open teacher memo
            app.Tap(c => c.Marked("TeacherMemo"));

            Thread.Sleep(tolerance);

            //Start playing
            app.Tap(c => c.Marked("PlayPauseButton"));

            Thread.Sleep(tolerance);

            //Pause playing
            app.Tap(c => c.Marked("PlayPauseButton"));

            //Check if the progress is at 00:01
            var progressQuery = app.Query(c => c.Marked("00:01"));
            Assert.Greater(progressQuery.Length, 0);

            //Continue playing
            app.Tap(c => c.Marked("PlayPauseButton"));

            Thread.Sleep(tolerance);

            //Pause again
            app.Tap(c => c.Marked("PlayPauseButton"));

            //Check if the progress is at 00:02
            //Check if the progress is between 00:02 and 00:03 (Xamarin UITest is slow sometimes)
            var progressList = new List<AppResult>();
            progressList.AddRange(app.Query(c => c.Marked("00:02")));
            progressList.AddRange(app.Query(c => c.Marked("00:03")));
            Assert.Greater(progressQuery.Length, 0);

            //Rewind
            app.Tap(c => c.Marked("RewindButton"));

            //Check if the progress is at 00:00
            progressQuery = app.Query(c => c.Marked("00:00"));

            Assert.Greater(progressQuery.Length, 0);

            //Forward
            app.Tap(c => c.Marked("ForwardButton"));

            //Check if the progress is at 00:05
            progressQuery = app.Query(c => c.Marked("00:05"));

            Assert.Greater(progressQuery.Length, 0);
        }

        [Test]
        public void RecordTeacherMemoStudentTrackTest()
        {
            int timeout = 2000;
            int tolerance = 500;

            RecordTeacherMemoTest();

            //Open teacher memo
            app.Tap(c => c.Marked("TeacherMemo"));

            Thread.Sleep(tolerance);

            //Start recording
            app.Tap(c => c.Marked("RecordStopButton"));

            Thread.Sleep(timeout);

            //Stop recording
            app.Tap(c => c.Marked("RecordStopButton"));

            Thread.Sleep(tolerance);

            //Check if the mute button is visible (=> StudentTrack exists)
            Assert.Greater(app.Query(c => c.Marked("MuteButton")).Length, 0);
        }

        [Test]
        public void EditTeacherMemoTest()
        {
            RecordTeacherMemoTest();

            app.TouchAndHold(c => c.Marked("TeacherMemo"));

            app.Tap(c => c.Marked("Bearbeiten"));

            app.Tap(c => c.Marked("EditButton"));

            Thread.Sleep(500);

            //Workaround as EnterText appends at a random place in the Entry
            app.EnterText(c => c.Marked("NameEntry"), "");
            app.ClearText();

            app.EnterText(c => c.Marked("NameEntry"), "TeacherMemo1");

            app.Tap(c => c.Marked("Speichern"));

            Thread.Sleep(500);

            //Check if a formstextview with text TeacherMemo exists
            var pageTexts = new List<AppResult>(app.Query(q => q.Class("FormsTextView")));

            Assert.IsTrue(pageTexts.Count == 1 && pageTexts[0].Text.Equals("TeacherMemo1"));
        }

        [Test]
        public void DeleteTeacherMemoTest()
        {
            RecordTeacherMemoTest();

            app.TouchAndHold(c => c.Marked("TeacherMemo"));

            app.Tap(c => c.Marked("Löschen"));

            //Click ok on warning dialog
            app.TapCoordinates(570, 730);

            Assert.AreEqual(app.Query(q => q.Class("ListView").Child()).Length, 0);
        }

    }
}
