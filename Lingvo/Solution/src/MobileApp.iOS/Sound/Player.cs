using System;
using AVFoundation;
using Foundation;
using Lingvo.Common.Adapters;
using Lingvo.Common.Entities;
using Lingvo.Common.Enums;
using Lingvo.MobileApp.iOS.Sound;
using Xamarin.Forms;
using System.Timers;

[assembly: Dependency(typeof(Player))]
namespace Lingvo.MobileApp.iOS.Sound
{
	public class Player : IPlayer
	{
		
		private AVAudioPlayer teacherTrack;
		private AVAudioPlayer studentTrack;
		private Timer timer;
		private PlayerState state;
		public event Action<int> Update;
		public event Action<PlayerState> StateChange;

		public Player()
		{
			timer = new Timer(100);
			timer.AutoReset = true;
			timer.Elapsed += (sender, e) => OnProgress();
			State = PlayerState.IDLE;

			//Initialize audio session
			ActivateAudioSession();
		}

		#region Public properties

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

		public bool IsStudentTrackMuted
		{
			get
			{
				return (studentTrack.Volume.Equals(0.0f));
			}

			set
			{
				studentTrack.Volume = value ? 0.0f : 1.0f;   
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
				return 0;
			}
		}

		public double CurrentProgress
		{
			get
			{
				if (teacherTrack != null)
				{
					return teacherTrack.CurrentTime;
				}
				return 0;
			}
		}


		#endregion

		#region Controls presented by interface

		public void Play()
		{
			if (studentTrack != null)
			{
				teacherTrack.Play();
				studentTrack.Play();
			}
			else
			{
				teacherTrack.Play();
			}
			State = PlayerState.PLAYING;
			timer.Start();
		}

		public void Pause()
		{
			if (studentTrack != null)
			{
				teacherTrack.Pause();
				studentTrack.Pause();
				Sync();
			}
			else
			{
				teacherTrack.Pause();
			}
			State = PlayerState.PAUSED;
			timer.Stop();

		}

		public void Stop()
		{
			timer.Stop();
			State = PlayerState.STOPPED;
			if (studentTrack != null)
			{
				teacherTrack.Stop();
				studentTrack.Stop();
				teacherTrack.CurrentTime = 0;
				studentTrack.CurrentTime = 0;
			}
			else
			{
				teacherTrack.Stop();
				teacherTrack.CurrentTime = 0;
			}
				

		}


		public void SeekTo(int seconds)
		{
			if (teacherTrack.CurrentTime + seconds > teacherTrack.Duration)
			{
				seconds = (int)teacherTrack.Duration;
				teacherTrack.CurrentTime = seconds;
				OnProgress();
				Stop();
				return;
			}

			if (studentTrack != null)
			{
				teacherTrack.CurrentTime += (double)seconds;
				studentTrack.CurrentTime += (double)seconds;
			}
			else
			{
				teacherTrack.CurrentTime += (double)seconds;	
			}
			OnProgress();
		}

		public void PrepareTeacherTrack(Recording recording)
		{
			NSUrl url = NSUrl.FromString(recording.LocalPath);
			teacherTrack = AVAudioPlayer.FromUrl(url);
			teacherTrack.PrepareToPlay();
			teacherTrack.FinishedPlaying += (sender, e) => Stop();
			State = PlayerState.STOPPED;
		}

		public void PrepareStudentTrack(Recording recording)
		{
			NSUrl url = NSUrl.FromString(recording.LocalPath);
			studentTrack = AVAudioPlayer.FromUrl(url);
			studentTrack.PrepareToPlay();
		}

		#endregion

		//The following methods manage the correct behaviour for 
		//the audio playback when the app enters the background 
		#region Session managing sessions

		public void ActivateAudioSession()
		{
			var session = AVAudioSession.SharedInstance();
			session.SetCategory(AVAudioSessionCategory.Ambient);
			session.SetActive(true);
		}

		public void DeactivateAudioSession()
		{
			var session = AVAudioSession.SharedInstance();
			session.SetActive(false);
		}

		public void ReactivateAudioSession()
		{
			var session = AVAudioSession.SharedInstance();
			session.SetActive(true);
		}

		#endregion

		/// <summary>
		/// This method is called via the elapsed timer to create a DrawUpdate for the view
		/// with the current playback progess in milliseconds
		/// </summary>
		private void OnProgress()
		{
			var milliseconds = teacherTrack.CurrentTime * 1000;

			Update?.Invoke((int)milliseconds);

		}

		private void OnStateChange()
		{
			StateChange?.Invoke(state);
		}

		private void Sync()
		{
			studentTrack.CurrentTime = teacherTrack.CurrentTime;
		}



	}
}
