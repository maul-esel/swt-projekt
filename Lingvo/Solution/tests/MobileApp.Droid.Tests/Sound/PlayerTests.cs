using NUnit.Framework;
using System.IO;
using System;
using System.Threading;
using Lingvo.Common.Entities;
using Lingvo.Common.Enums;
using Lingvo.MobileApp.Droid.Sound;
using Android.App;
using System.Reflection;

namespace MobileApp.Droid.Tests
{
    [TestFixture]
    public class PlayerTests
    {
        private const String testFileName = "test_audio_file.mp3";
        private String testFilePath;
        private int testFileDuration = 370000; //Actual milliseconds of the test audio file

        private Recording testRecording;
        private Player testPlayer;

        //This method is executed once before all tests are performed
        [TestFixtureSetUp]
        public void Init()
        {
            //Creating a test audiofile

            //Read the audio test file from resources bundle
            var context = Application.Context;
            var assets = context.Assets;
            var fileStream = assets.Open(testFileName);

            //Create filePath to default documents directory
            var documentsDirPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            testFilePath = Path.Combine(documentsDirPath, testFileName);

            ////Now write the testfile to the documents directory
            var testFileStream = File.Create(testFilePath);
            fileStream.CopyTo(testFileStream);

            //Create a test teacher track recording
            testRecording = new Recording();
            testRecording.LocalPath = testFilePath;
            testRecording.Duration = testFileDuration;

        }

        //This method is executed after all the unit tests have been performed
        [TestFixtureTearDown]
        public void TearDown()
        {
            //Delete the test audio recording
            File.Delete(testFilePath);
        }

        [SetUp]
        public void SetupBeforeEachTest()
        {
            testPlayer = new Player();

            testPlayer.PrepareTeacherTrack(testRecording);
        }

        [TearDown]
        public void cleanUpAfterEachTest()
        {
            testPlayer.Stop();
            testPlayer = null;
        }

        //Unit tests

        [Test]
        public void ShouldBePrepared()
        {
            Assert.IsTrue(testPlayer.State == PlayerState.STOPPED);
        }

        [Test]
        public void ShouldBePlaying()
        {
            testPlayer.Play();
            Assert.IsTrue(testPlayer.State == PlayerState.PLAYING);
        }

        [Test]
        public void ShouldBePaused()
        {
            testPlayer.Pause();
            Assert.AreEqual(PlayerState.PAUSED, testPlayer.State);
        }

        [Test]
        public void ShouldResumeCorrectly()
        {
            testPlayer.Pause();
            var oldProgress = testPlayer.CurrentProgress;

            testPlayer.Play();
            var newProgress = testPlayer.CurrentProgress;

            Assert.IsTrue(newProgress >= oldProgress);
        }

        [Test]
        public void ShouldBeStopped()
        {
            testPlayer.Stop();
            Assert.AreEqual(PlayerState.STOPPED, testPlayer.State);
        }

        [Test]
        public void ShouldRestartCorrectly()
        {

            testPlayer.Play();
            Thread.Sleep(3000); //Let the player play for some time.

            testPlayer.Stop();

            Assert.AreEqual(0, testPlayer.CurrentProgress); //Player should have reset its progress.

            testPlayer.Play();

            Assert.IsTrue(testPlayer.CurrentProgress < 1000 && testPlayer.State == PlayerState.PLAYING);

        }

        [Test]
        public void ShouldSkipCorrectly()
        {
            testPlayer.Play();
            testPlayer.Pause(); //These two lines create the real situation in which a skipping is possible

            var currentProgress = testPlayer.CurrentProgress;

            testPlayer.SeekTo(5);
            testPlayer.SeekTo(-5);

            Assert.AreEqual(currentProgress, testPlayer.CurrentProgress);

            testPlayer.SeekTo(5);
            testPlayer.SeekTo(5);
            testPlayer.SeekTo(5);

            Assert.AreEqual(currentProgress + 15000, testPlayer.CurrentProgress);

        }


    }
}
