using System;
using Android.Media;
using Lingvo.Common.Adapters;
using Lingvo.Common.Entities;
using Lingvo.Common.Enums;
using Lingvo.MobileApp.Droid.Sound;
using Xamarin.Forms;

[assembly: Dependency(typeof(Player))]
namespace Lingvo.MobileApp.Droid.Sound
{
	public class Player : IPlayer
	{

		private MediaPlayer teacherTrack;
		private MediaPlayer studentTrack;

		private bool isStudentTrackMuted;

		public event Action<int> Update;
		public event Action<PlayerState> StateChange;

		public Player()
		{
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
				float leftVolume = value ? 0.0f : 1.0f;
				float rightVolume = value ? 0.0f : 1.0f;

				studentTrack.SetVolume(leftVolume, rightVolume);
			}
		}

		public double TotalLengthOfTrack
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public double CurrentProgress
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public PlayerState State
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void Continue()
		{
			Play();
		}

		public void Pause()
		{
			teacherTrack.Pause();
			if (studentTrack != null)
			{
				studentTrack.Pause();
			}
		}

		public void Play()
		{
			teacherTrack.Start();
			if (studentTrack != null)
			{
				studentTrack.Start();
			}
		}

		public void PrepareStudentTrack(Recording recording)
		{
			studentTrack = CreateNewPlayer(recording);
			IsStudentTrackMuted = false;
		}

		public void PrepareTeacherTrack(Recording recording)
		{
			teacherTrack = CreateNewPlayer(recording);
		}

		public void SeekTo(TimeSpan seek)
		{
			teacherTrack.SeekTo(seek.Milliseconds);
		}

		public void Stop()
		{
			teacherTrack.Stop();
			if (studentTrack != null)
			{
				studentTrack.Stop();
			}

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

		public void SeekTo(int seek)
		{
			throw new NotImplementedException();
		}
	}
}
