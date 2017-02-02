using System;
using System.IO;
using Xamarin.Forms;
using Foundation;
using Lingvo.Common.Adapters;
using Lingvo.MobileApp.iOS.Sound;
using Lingvo.Common.Enums;
using Lingvo.Common.Entities;
using Lingvo.Common.Services;
using AVFoundation;

[assembly: Dependency(typeof(Recorder))]
namespace Lingvo.MobileApp.iOS.Sound
{

	public class Recorder :IRecorder
	{
		private const String RECORDING_PREFIX = "record_";
		private const String DATE_FORMAT = "yyyy-MM-ddTHH-mm-ss";
		private NSUrl currentRecordingUrl;
		private Recording currentRecording;
		private AVAudioRecorder recorder;
		private NSError error;

		private static readonly AudioSettings SETTINGS = new AudioSettings
		{
			SampleRate = 44100.0f,
			Format = AudioToolbox.AudioFormatType.MPEG4AAC,
			NumberChannels = 1,
			LinearPcmBitDepth = 16,
			AudioQuality = AVAudioQuality.High,
		};

		private RecorderState state;


		public Recorder()
		{
			if (!initAudioSession())
			{
				State = RecorderState.ERROR;
			}
			State = RecorderState.IDLE;
		}

		public RecorderState State
		{
			get
			{
				return state;
			}

			private set
			{
				state = value;
			}
				
		}

		public void Continue()
		{
			recorder.Record();
			State = RecorderState.RECORDING;
		}

		public void Pause()
		{
			if (State == RecorderState.RECORDING)
			{
				recorder.Pause();
				State = RecorderState.PAUSED;
			}

		}

		public void Start()
		{
			recorder.Record();
			State = RecorderState.RECORDING;
		}

		public Recording Stop()
		{
			recorder.Pause();
			int recordingDuration = (int) recorder.currentTime;
			recorder.Stop();

			recorder = null;
			State = RecorderState.IDLE;

			currentRecording = new Recording();
			currentRecording.LocalPath = Path.GetFileName(currentRecordingUrl.Path);
			currentRecording.Duration = recordingDuration;

			return currentRecording;
		}

		public bool PrepareToRecord()
		{
			currentRecordingUrl = NSUrl.FromFilename(FileUtil.getAbsolutePath(getFileName()));
			recorder = AVAudioRecorder.Create(currentRecordingUrl, SETTINGS, out error);


			if (error != null)
			{
				State = RecorderState.ERROR;
				return false;
			}

			//Set Recorder to Prepare To Record
			if (!recorder.PrepareToRecord())
			{
				
				recorder.Dispose();
				recorder = null;
				State = RecorderState.ERROR;
				return false;
			}


			State = RecorderState.PREPARED;
			return true;

		}

		private bool initAudioSession()
		{
			var audioSession = AVAudioSession.SharedInstance();
			var err = audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord);
			if (err != null)
			{
				return false;
			}
			err = audioSession.SetActive(true);
			if (err != null)
			{
				return false;
			}
			return true;
		}

		private String getFileName()
		{
			return RECORDING_PREFIX + DateTime.Now.ToString(DATE_FORMAT) + ".aac";
		}
	}
}
