using System;
using AVFoundation;
using Foundation;
using Lingvo.Common.Adapters;
using Lingvo.Common.Entities;
using Lingvo.MobileApp.iOS.Sound;
using Xamarin.Forms;

[assembly: Dependency(typeof(Player))]
namespace Lingvo.MobileApp.iOS.Sound
{
	public class Player : IPlayer
	{
		private AVAudioPlayer teacherTrack;
		private AVAudioPlayer studentTrack;


		public Player()
		{
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


		#endregion

		#region Controls presented by interface

		public void Play()
		{
			teacherTrack.Play();
			if (studentTrack != null)
			{
				studentTrack.Play();
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

	}
}
