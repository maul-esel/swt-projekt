using System;
using System.IO.IsolatedStorage;
using AVFoundation;
using Foundation;
using Lingvo.Common.Adapters;
using Lingvo.Common.Entities;
using Lingvo.Common.Enums;
using Lingvo.Common.Services;
using Lingvo.MobileApp.iOS.Sound;
using Xamarin.Forms;
using System.Timers;
using CoreFoundation;
using System.Threading.Tasks;

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
				if (studentTrack != null)
				{
					return (studentTrack.Volume.Equals(0.0f));
				}
				return true;

			}

			set
			{
				if (studentTrack != null)
				{
					studentTrack.Volume = value ? 0.0f : 1.0f;
				}

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

			AVAudioSession.SharedInstance().SetActive(true);
			if (studentTrack != null)
			{
				studentTrack.Volume = 1.0f;
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

				studentTrack.Volume = 0.0f;
			}
			else
			{
				teacherTrack.Stop();
				teacherTrack.CurrentTime = 0;
			}
	
			OnStateChange();
			//workaround: kill audio session because AVAudioPlayer keeps playing when you call .Stop()
			Task.Run(() => AVAudioSession.SharedInstance().SetActive(false));



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
			NSUrl url = NSUrl.FromString(FileUtil.getAbsolutePath(recording));
			teacherTrack = AVAudioPlayer.FromUrl(url);
			teacherTrack.PrepareToPlay();
			teacherTrack.FinishedPlaying += (sender, e) => Stop();
			State = PlayerState.STOPPED;
		}

		public void PrepareStudentTrack(Recording recording)
		{
			if (recording == null)
			{
				studentTrack = null;
			}
			else
			{
				NSUrl url = NSUrl.FromString(FileUtil.getAbsolutePath(recording));
				studentTrack = AVAudioPlayer.FromUrl(url);
				studentTrack.PrepareToPlay();
			}

		}

		#endregion

		//The following methods manage the correct behaviour for 
		//the audio playback when the app enters the background 
		#region Session managing sessions

		public void ActivateAudioSession()
		{
			var status = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Audio);
			AVAudioSession session = AVAudioSession.SharedInstance();
			session.SetCategory(AVAudioSessionCategory.PlayAndRecord);
			NSError err;
			session.OverrideOutputAudioPort(AVAudioSessionPortOverride.Speaker, out err);
			session.SetActive(true);
			if (status == AVAuthorizationStatus.NotDetermined)
			{
				session.RequestRecordPermission((granted) => { });
			}

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
