using System;
using Android.Media;
using Lingvo.Common.Adapters;
using Lingvo.Common.Entities;
using Lingvo.Common.Enums;
using Lingvo.MobileApp.Droid.Sound;
using Xamarin.Forms;
using Android.OS;

[assembly: Dependency(typeof(Player))]
namespace Lingvo.MobileApp.Droid.Sound
{
    public class Player : IPlayer
    {

        private MediaPlayer teacherTrack;
        private MediaPlayer studentTrack;

        private bool isStudentTrackMuted;

        private PlayerState state;

        public event Action<int> Update;
        public event Action<PlayerState> StateChange;

        private Handler progressHandler;
        private Action progressUpdate; 

    public Player()
		{
            progressHandler = new Handler(Looper.MainLooper);
            progressUpdate = new Action(() =>
            {
                OnProgress();
                
                if(studentTrack.CurrentPosition > teacherTrack.CurrentPosition + 100)
                {
                    Sync();
                }

                progressHandler.PostDelayed(progressUpdate, 100);
            });
            State = PlayerState.IDLE;
        }

        private void OnProgress()
        {
            var milliseconds = teacherTrack.CurrentPosition * 1000;
            Update?.Invoke((int)milliseconds);

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
				studentTrack.SetVolume(volume, volume);
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
			studentTrack = CreateNewPlayer(recording);
			IsStudentTrackMuted = false;
		}

		public void PrepareTeacherTrack(Recording recording)
		{
			teacherTrack = CreateNewPlayer(recording);
            teacherTrack.Completion += (o, e) => Stop();
            State = PlayerState.STOPPED;
		}

		public void SeekTo(int milliseconds)
		{
            if(teacherTrack?.CurrentPosition + milliseconds >= teacherTrack?.Duration)
            {
                teacherTrack?.SeekTo(teacherTrack.Duration);
                OnProgress();
                Stop();
                return;
            }
			teacherTrack?.SeekTo(milliseconds);
            studentTrack?.SeekTo(milliseconds);
            OnProgress();
		}

		public void Stop()
		{
			teacherTrack?.Stop();
			studentTrack?.Stop();
            teacherTrack?.SeekTo(0);
            studentTrack.SeekTo(0);
            State = PlayerState.STOPPED;
            progressHandler.RemoveCallbacks(progressUpdate);
		}

		private MediaPlayer CreateNewPlayer(Recording recording)
		{
			var mediaPlayer = new MediaPlayer();

			var file = global::Android.App.Application.Context.Assets.OpenFd(recording.LocalPath);
			mediaPlayer.SetDataSource(file.FileDescriptor, file.StartOffset, file.Length);
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
