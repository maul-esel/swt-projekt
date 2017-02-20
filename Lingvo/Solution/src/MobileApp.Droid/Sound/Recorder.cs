using System;
using System.IO;
using Xamarin.Forms;
using Lingvo.Common.Adapters;
using Lingvo.Common.Enums;
using Lingvo.Common.Entities;
using Lingvo.Common.Services;
using Android.Media;
using Lingvo.MobileApp.Droid.Sound;

[assembly: Dependency(typeof(Recorder))]
namespace Lingvo.MobileApp.Droid.Sound
{

    public class Recorder : IRecorder
    {
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
            if (State == RecorderState.PAUSED)
            {
                recorder.Resume();
                State = RecorderState.RECORDING;
            }
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

        public bool PrepareToRecord()
        {
            currentRecordingPath = FileUtil.getAbsolutePath(getFileName());
            recorder = new MediaRecorder();

            audioManager.Mode = audioManager.WiredHeadsetOn ? Mode.Normal : Mode.InCommunication;

            recorder.SetAudioSource(audioManager.WiredHeadsetOn ? AudioSource.Mic : AudioSource.VoiceCommunication);
            recorder.SetOutputFormat(OutputFormat.ThreeGpp);
            recorder.SetAudioEncoder(AudioEncoder.AmrNb);
            recorder.SetOutputFile(currentRecordingPath);

            recorder.Prepare();
            State = RecorderState.PREPARED;
            return true;

        }

        private String getFileName()
        {
            return RECORDING_PREFIX + DateTime.Now.ToString(DATE_FORMAT) + ".3gpp";
        }
    }
}
