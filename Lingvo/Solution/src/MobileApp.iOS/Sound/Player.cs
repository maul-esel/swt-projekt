using System;
using AVFoundation;
using Foundation;
using Lingvo.Common.Adapters;
using Lingvo.Common.Entities;
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


		public event Action<int> Update;

		public Player()
		{
			timer = new Timer(100);
			timer.AutoReset = true;
			timer.Enabled = true;
			timer.Elapsed += (sender, e) => OnUpdate();


			//Initialize audio session
			ActivateAudioSession();
		}
		#region Public properties

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
			teacherTrack.Play();
			if (studentTrack != null)
			{
				studentTrack.Play();
			}
			timer.Start();
		}


		public void Pause()
		{
			teacherTrack.Pause();
			if (studentTrack != null)
			{
				studentTrack.Pause();
			}
			timer.Stop();

		}

		public void SeekTo(TimeSpan timeCode)
		{
			teacherTrack.CurrentTime += (double)timeCode.Seconds;
		}

		public void Stop()
		{
			teacherTrack.Stop();

			teacherTrack.CurrentTime = 0;
			if (studentTrack != null)
			{
				studentTrack.Stop();
			}
			timer.Stop();
		}

		public void PrepareTeacherTrack(Recording recording)
		{
			NSUrl url = NSUrl.FromString(recording.LocalPath);
			teacherTrack = AVAudioPlayer.FromUrl(url);
			teacherTrack.PrepareToPlay();
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
		private void OnUpdate()
		{
			var milliseconds = teacherTrack.CurrentTime * 1000;

			Update?.Invoke((int)milliseconds);

		}

	}
}
