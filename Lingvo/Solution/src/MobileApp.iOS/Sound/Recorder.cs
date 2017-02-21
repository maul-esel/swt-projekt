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
		//Prefix constants for the recording names
		private const String RECORDING_PREFIX = "record_";
		private const String DATE_FORMAT = "yyyy-MM-ddTHH-mm-ss";

		private NSUrl currentRecordingUrl;
		private Recording currentRecording;
		private AVAudioRecorder recorder;
		private NSError error;

		//The settings for the recorder which are used for new recordings
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
				//This means we don't have acess to a microphone and cannot record
				//So we are entering error state
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

		/// <summary>
		/// Continues a currently running recording at the current position.
		/// </summary>
		public void Continue()
		{
			recorder.Record();
			State = RecorderState.RECORDING;
		}

		/// <summary>
		/// Pauses a currently running recording. This is the counterpart to
		/// the continue method.
		/// </summary>
		public void Pause()
		{
			if (State == RecorderState.RECORDING)
			{
				recorder.Pause();
				State = RecorderState.PAUSED;
			}

		}


		/// <summary>
		/// Starting the recorder
		/// </summary>
		public void Start()
		{
			recorder.Record();
			State = RecorderState.RECORDING;
		}


		/// <summary>
		/// Stopping the current recording session and creating a new Recording object.
		/// </summary>
		/// <returns>The new recording</returns>
		public Recording Stop()
		{
			recorder.Pause();

			//Converting from seconds to milliseconds
			int recordingDuration = (int) (recorder.currentTime * 1000);

			recorder.Stop();

			recorder = null;
			State = RecorderState.IDLE;


			currentRecording = new Recording();
			currentRecording.LocalPath = Path.GetFileName(currentRecordingUrl.Path);
			currentRecording.Duration = recordingDuration;

			return currentRecording;
		}

		/// <summary>
		/// Preparing the recorder. has to be called before a recording can be started.
		/// </summary>
		/// <returns><c>true</c>, if to record was prepared, <c>false</c> otherwise.</returns>
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

		/// <summary>
		/// Creating an audio session for playback and recording
		/// </summary>
		/// <returns><c>true</c>, if audio session was inited, <c>false</c> otherwise.</returns>
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

		/// <summary>
		/// Creates a new filename for a new recording with current daytime.
		/// </summary>
		/// <returns>The file name.</returns>
		private String getFileName()
		{
			return RECORDING_PREFIX + DateTime.Now.ToString(DATE_FORMAT) + ".aac";
		}
	}
}
