using NUnit.Framework;
using System.IO;
using System;
using System.Threading;
using Foundation;
using Lingvo.Common.Entities;
using Lingvo.Common.Enums;
using Lingvo.MobileApp.iOS.Sound;

namespace MobileApp.iOS.Tests
{
	[TestFixture]
	public class RecorderTests
	{
		private const int timeout = 2000;
		private const int tolerance = 200;
		private Recorder testRecorder;

		//This method is called before each test
		[SetUp]
		public void Init()
		{
			testRecorder = new Recorder();
		}

		//This method is called after each test
		[TestFixtureTearDown]
		public void TearDown()
		{
			testRecorder.Stop();
			testRecorder = null;
		}


		[Test]
		public void ShouldBePrepared()
		{
			testRecorder.PrepareToRecord();
			Assert.AreEqual(RecorderState.PREPARED,testRecorder.State);
		}

		[Test]
		public void ShouldBeRecording()
		{
			testRecorder.PrepareToRecord();
			testRecorder.Start();

			Assert.AreEqual(RecorderState.RECORDING, testRecorder.State);
		}

		[Test]
		public void ShouldDeliverRecording()
		{
			testRecorder.PrepareToRecord();
			testRecorder.Start();

			Thread.Sleep(timeout); //Let the recorder record for a while

			Recording resultTrack = testRecorder.Stop();


			Assert.IsTrue(resultTrack != null);

			//The recording should have a duration of approximately the value of timeout
			//tolerance of 100ms is given as it is more than unlikely that the duration would
			//be EXACTLY timeout-value
			Assert.IsTrue(resultTrack.Duration > timeout - tolerance);
			Assert.IsTrue(resultTrack.Duration < timeout + tolerance);
		}

		[Test]
		public void ShouldContinueCorrectly()
		{
			testRecorder.PrepareToRecord();
			testRecorder.Start();

			Thread.Sleep(timeout); //Let the recorder record for a while
			testRecorder.Pause();

			Thread.Sleep(timeout); // During this time the recorder should not be recording.
			testRecorder.Continue();

			Thread.Sleep(timeout); //Let the recorder record for a while again
			Recording resultTrack = testRecorder.Stop();

			//Now the total length of the recording should not 
			//be longer than approximately 2*timeout


			Assert.IsTrue(resultTrack.Duration < 2 * (timeout + tolerance));
			Assert.IsTrue(resultTrack.Duration > 2 * (timeout - tolerance));

		}
	}
}
