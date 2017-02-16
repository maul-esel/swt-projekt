using System;
using Android.Media;
using Lingvo.Common.Adapters;
using Lingvo.Common.Entities;
using Lingvo.Common.Enums;
using Lingvo.MobileApp.Droid.Sound;
using Xamarin.Forms;
using Android.OS;
using Lingvo.Common.Services;

[assembly: Dependency(typeof(Player))]
namespace Lingvo.MobileApp.Droid.Sound
{
    public class Player : IPlayer
    {

        private MediaPlayer teacherTrack;
        private MediaPlayer studentTrack;

        private Recording teacherRecording;
        private Recording studentRecording;

        private bool isStudentTrackMuted;

        private PlayerState state;

        public event Action<int> Update;
        public event Action<PlayerState> StateChange;

        private Handler progressHandler;
        private Action progressUpdate;

        private AudioManager audioManager;

        public Player()
        {
            audioManager = AudioManager.FromContext(Android.App.Application.Context);
            progressHandler = new Handler(Looper.MainLooper);
            progressUpdate = new Action(() =>
            {
                if (State == PlayerState.PLAYING)
                {
                    OnProgress(teacherTrack.CurrentPosition);
                    progressHandler.PostDelayed(progressUpdate, 100);
                }
            });
            State = PlayerState.IDLE;
        }

        private void OnProgress(int elapsedMilliseconds)
        {
			Update?.Invoke(elapsedMilliseconds);

        }

        public bool IsStudentTrackMuted
        {
            get
            {
                return isStudentTrackMuted;
            }

            set
            {
                isStudentTrackMuted = value;
                float volume = isStudentTrackMuted ? 0.0f : 1.0f;
                studentTrack?.SetVolume(volume, volume);
            }
        }

        public double TotalLengthOfTrack
        {
            get
            {
                if (teacherTrack != null)
                {
                    return teacherTrack.Duration;
                }
                return 0.0;
            }
        }

        public double CurrentProgress
        {
            get
            {
                if (teacherTrack != null)
                {
                    return teacherTrack.CurrentPosition;
                }
                return 0.0;
            }
        }

        public PlayerState State
        {
            get
            {
                return state;
            }
            private set
            {
                state = value;
                OnStateChange();
            }
        }

        public void Pause()
        {
            teacherTrack?.Pause();
            studentTrack?.Pause();
            State = PlayerState.PAUSED;
            progressHandler.RemoveCallbacks(progressUpdate);
            Sync();
        }

        public void Play()
        {
            teacherTrack?.Start();
            studentTrack?.Start();
            State = PlayerState.PLAYING;
            progressHandler.PostDelayed(progressUpdate, 100);
        }

        public void PrepareStudentTrack(Recording recording)
        {
			if (recording == null)
			{
				studentTrack = null;
			}
			else
			{
				studentRecording = recording;
				studentTrack = CreateNewPlayer(recording);
				IsStudentTrackMuted = false;
			}
        }

        public void PrepareTeacherTrack(Recording recording)
        {
            teacherRecording = recording;
            teacherTrack = CreateNewPlayer(recording);
			teacherTrack.Completion += (o, e) =>
			{
				Stop();
				OnProgress(teacherTrack.Duration);
			};
            State = PlayerState.STOPPED;
			OnProgress(teacherTrack.CurrentPosition);
        }

        public void SeekTo(int seconds)
        {
            if (teacherTrack?.CurrentPosition + seconds * 1000 >= teacherTrack?.Duration)
            {
                teacherTrack?.SeekTo(teacherTrack.Duration);
                OnProgress(teacherTrack.CurrentPosition);
                Stop();
                return;
            }
            teacherTrack?.SeekTo(teacherTrack.CurrentPosition + seconds * 1000);
            studentTrack?.SeekTo(studentTrack.CurrentPosition + seconds * 1000);
            OnProgress(teacherTrack.CurrentPosition);
        }

        public void Stop()
        {
            teacherTrack?.Stop();
            studentTrack?.Stop();
            ReinitializePlayer(teacherTrack, teacherRecording);
            ReinitializePlayer(studentTrack, studentRecording);
            State = PlayerState.STOPPED;
            progressHandler.RemoveCallbacks(progressUpdate);
        }

        private void ReinitializePlayer(MediaPlayer player, Recording recording)
        {
            if (recording != null)
            {
                player?.Reset();
                var fileDesriptor = Android.OS.ParcelFileDescriptor.Open(new Java.IO.File(FileUtil.getAbsolutePath(recording)), Android.OS.ParcelFileMode.ReadOnly);
                player?.SetDataSource(fileDesriptor.FileDescriptor);
                //var file = global::Android.App.Application.Context.Resources.OpenRawResourceFd(Resource.Raw.sound);
                //player?.SetDataSource(file.FileDescriptor, file.StartOffset, file.Length);
                player?.Prepare();
            }
        }

        private MediaPlayer CreateNewPlayer(Recording recording)
        {
            var mediaPlayer = new MediaPlayer();

            audioManager.SpeakerphoneOn = !audioManager.WiredHeadsetOn;
            audioManager.Mode = audioManager.WiredHeadsetOn ? Mode.Normal : Mode.InCommunication;


			var fileDesriptor = Android.OS.ParcelFileDescriptor.Open(new Java.IO.File(FileUtil.getAbsolutePath(recording)), Android.OS.ParcelFileMode.ReadOnly);
                mediaPlayer.SetDataSource(fileDesriptor.FileDescriptor);
           
            mediaPlayer.SetAudioStreamType(audioManager.WiredHeadsetOn ? Stream.Music : Stream.VoiceCall);
            mediaPlayer.SetVolume(1.0f, 1.0f);
            mediaPlayer.Prepare();

            return mediaPlayer;
        }

        private void OnStateChange()
        {
            StateChange?.Invoke(state);
        }

        private void Sync()
        {
            studentTrack?.SeekTo(teacherTrack.CurrentPosition);
        }
    }
}
