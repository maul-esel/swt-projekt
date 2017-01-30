using System;
using System.IO;
using Xamarin.Forms;
using Lingvo.Common.Adapters;
using Lingvo.Common.Enums;
using Lingvo.Common.Entities;
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
                recorder.Reset();

                //Hack: For duration, a new player is needed
                MediaPlayer tempPlayer = new MediaPlayer();
                var fileDesriptor = Android.OS.ParcelFileDescriptor.Open(new Java.IO.File(currentRecordingPath), Android.OS.ParcelFileMode.ReadOnly);
                tempPlayer.SetDataSource(fileDesriptor.FileDescriptor);
                tempPlayer.Prepare();
                int recordingDuration = tempPlayer.GetTrackInfo().Length;
                tempPlayer.Release();

                recorder = null;
                State = RecorderState.IDLE;

                //TODO: Issue! Where does the id come from? Could use other constructor but then the property setter have to be set to public
                currentRecording = new Recording(37, recordingDuration, currentRecordingPath, DateTime.Now);

                return currentRecording;
            }
            return null;
        }

        public bool PrepareToRecord()
        {
            currentRecordingPath = Path.Combine(getFilePath(), getFileName());
            recorder = new MediaRecorder();

            recorder.SetAudioSource(audioManager.WiredHeadsetOn ? AudioSource.Mic : AudioSource.VoiceCommunication);
            recorder.SetOutputFormat(OutputFormat.ThreeGpp);
            recorder.SetAudioEncoder(AudioEncoder.AmrNb);
            recorder.SetAudioSamplingRate(44100);
            recorder.SetAudioEncodingBitRate(16);
            recorder.SetOutputFile(currentRecordingPath);
            recorder.Prepare();

            State = RecorderState.PREPARED;
            return true;

        }

        private String getFilePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        private String getFileName()
        {
            return RECORDING_PREFIX + DateTime.Now.ToString(DATE_FORMAT) + ".3gpp";
        }
    }
}
