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
        private const String DATE_FORMAT = "yyyy-MM-ddTHH:mm:ss";
        private string currentRecordingPath;
        private Recording currentRecording;
        private MediaRecorder recorder;
        private bool headsetConnected;
        private RecorderState state;

        public Recorder()
        {
            AudioManager manager = AudioManager.FromContext(Android.App.Application.Context);
            headsetConnected = manager.WiredHeadsetOn;
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
            recorder.Resume();
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

        public void SeekTo(int seconds)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            recorder.Start();
            State = RecorderState.RECORDING;
        }

        public Recording Stop()
        {
            recorder.Stop();
            recorder.Release();

            //Hack: For duration, a new player is needed
            MediaPlayer tempPlayer = new MediaPlayer();
            var file = global::Android.App.Application.Context.Assets.OpenFd(currentRecordingPath);
            tempPlayer.SetDataSource(file.FileDescriptor);
            tempPlayer.Prepare();
            int recordingDuration = tempPlayer.GetTrackInfo().Length;
            tempPlayer.Release();

            recorder = null;
            State = RecorderState.IDLE;

            //TODO: Issue! Where does the id come from? Could use other constructor but then the property setter have to be set to public
            currentRecording = new Recording(37, recordingDuration, currentRecordingPath, DateTime.Now);

            return currentRecording;
        }

        public bool PrepareToRecord()
        {
            currentRecordingPath = Path.Combine(getFilePath(), getFileName());
            recorder = new MediaRecorder();

            try
            {
                recorder.SetAudioChannels(1);
                recorder.SetAudioEncoder(AudioEncoder.Aac);
                recorder.SetAudioSamplingRate(44100);
                recorder.SetAudioEncodingBitRate(16);
                recorder.SetAudioSource(headsetConnected ? AudioSource.Mic : AudioSource.VoiceCommunication);
                recorder.SetOutputFile(currentRecordingPath);
                recorder.Prepare();

                State = RecorderState.PREPARED;
                return true;
            } catch(Exception)
            {
                State = RecorderState.ERROR;
                return false;
            }
        }

        private String getFilePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        private String getFileName()
        {
            return RECORDING_PREFIX + DateTime.Now.ToString(DATE_FORMAT) + ".aac";
        }
    }
}
