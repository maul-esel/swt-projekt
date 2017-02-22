using System;
using System.IO;
using Xamarin.Forms;
using Lingvo.Common.Adapters;
using Lingvo.Common.Enums;
using Lingvo.Common.Entities;
using Lingvo.Common.Services;
using Android.Media;
using Lingvo.MobileApp.Droid.Sound;
using Android.OS;

[assembly: Dependency(typeof(Recorder))]
namespace Lingvo.MobileApp.Droid.Sound
{

    public class Recorder : IRecorder
    {
		//Prefix constants for the recording names
        private const String RECORDING_PREFIX = "record_";
        private const String DATE_FORMAT = "yyyy-MM-ddTHH-mm-ss";
        private string currentRecordingPath;
        private Recording currentRecording;
        private MediaRecorder recorder;
        private RecorderState state;
        private AudioManager audioManager;

        public Recorder()
        {
			
            audioManager = AudioManager.FromContext(Android.App.Application.Context);
            State = RecorderState.IDLE;
        }

		/// <summary>
		/// State property of the current recorder
		/// (idle, error, recording, stopped, paused)
		/// </summary>
		/// <value>The state.</value>
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
		/// Currently not used as pausing and resuming a recording is not supported
		/// before Android verion code 24 (Nougat)
		/// </summary>
        public void Continue()
        {
            if (State == RecorderState.PAUSED && Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                recorder.Resume();
                State = RecorderState.RECORDING;
            }
        }

		/// <summary>
		/// Currently not used as pausing and resuming a recording is not supported
		/// before Android verion code 24 (Nougat)
		/// </summary>
        public void Pause()
        {
            if (State == RecorderState.RECORDING && Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                recorder.Pause();
                State = RecorderState.PAUSED;
            }

        }

        public void Start()
        {
            recorder.Start();
            State = RecorderState.RECORDING;
        }

        public Recording Stop()
        {
            if (State == RecorderState.RECORDING || State == RecorderState.PAUSED)
            {
                recorder.Stop();
                recorder.Release();
                recorder.Dispose();

                MediaMetadataRetriever mmr = new MediaMetadataRetriever();
                mmr.SetDataSource(Android.App.Application.Context, Android.Net.Uri.FromFile(new Java.IO.File(currentRecordingPath)));
                String durationStr = mmr.ExtractMetadata(MediaMetadataRetriever.MetadataKeyDuration);
                int recordingDuration = int.Parse(durationStr);

                recorder = null;
                State = RecorderState.IDLE;

                currentRecording = new Recording();
                currentRecording.LocalPath = Path.GetFileName(currentRecordingPath);
                currentRecording.Duration = recordingDuration;

                return currentRecording;
            }
            return null;
        }

		/// <summary>
		/// Prepares the recording session, i.e. gets the correct
		/// audio input (headset or built in microphone) and output
		/// (Voice call for automatic noise cancelling)
		/// </summary>
		/// <returns><c>true</c>, if to record was prepared, <c>false</c> otherwise.</returns>
        public bool PrepareToRecord()
        {
            currentRecordingPath = FileUtil.getAbsolutePath(getFileName());
            recorder = new MediaRecorder();

            audioManager.SpeakerphoneOn = !audioManager.WiredHeadsetOn;
            audioManager.Mode = audioManager.WiredHeadsetOn ? Mode.Normal : Mode.InCommunication;

            audioManager.SetStreamVolume(Android.Media.Stream.VoiceCall, audioManager.GetStreamMaxVolume(Android.Media.Stream.VoiceCall), VolumeNotificationFlags.RemoveSoundAndVibrate);

            recorder.SetAudioSource(audioManager.WiredHeadsetOn ? AudioSource.Mic : AudioSource.VoiceCommunication);
            recorder.SetOutputFormat(OutputFormat.ThreeGpp);
            recorder.SetAudioEncoder(AudioEncoder.AmrNb);
            recorder.SetOutputFile(currentRecordingPath);

            recorder.Prepare();
            State = RecorderState.PREPARED;
            return true;

        }

		/// <summary>
		/// Returns a new filename following the specified filename pattern
		/// </summary>
		/// <returns>The file name.</returns>
        private String getFileName()
        {
            return RECORDING_PREFIX + DateTime.Now.ToString(DATE_FORMAT) + ".3gpp";
        }
    }
}
