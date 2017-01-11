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
		private AVAudioPlayer track;


		public Player()
		{
			//Initialize audio session
			ActivateAudioSession();
		}
	#region Public properties

		public bool IsMuted { get; set; } = false;
		public float Volume { get; set; } = 1.0f;

	#endregion

	#region Controls presented by interface
		public void Continue()
		{
			throw new NotImplementedException();
		}

		public void Pause()
		{
			track.Pause();

		}

		public void Play(Recording recording)
		{
			if (track == null)
			{
				NSUrl url = NSUrl.FromString(recording.LocalPath);
				track = AVAudioPlayer.FromUrl(url);
			}

			track.Play();
		}

		public void SeekTo(TimeSpan seek)
		{
			//TODO: Scrobbling
		}

		public void Stop()
		{
			track.Stop();
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
