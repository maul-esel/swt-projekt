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

		private const float teacherTrackVolume = 0.7f;
		private const float studentTrackVolume = 1.0f;

		public Player()
		{
			timer = new Timer(100);
			timer.AutoReset = true;
			timer.Elapsed += (sender, e) => OnProgress(teacherTrack.CurrentTime);
			State = PlayerState.IDLE;

			ActivateAudioSession();


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
					studentTrack.Volume = value ? 0.0f : studentTrackVolume;
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

		public void Play()
		{
			//Grabbing audio session
			AVAudioSession session = AVAudioSession.SharedInstance();
			adjustAudioOutputPort(session);
			AVAudioSession.SharedInstance().SetActive(true);

			if (studentTrack != null)
			{
				studentTrack.Volume = studentTrackVolume;
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
				OnProgress(teacherTrack.Duration);
				Stop();
				return;
			}
			teacherTrack.CurrentTime += (double)seconds;

			if (studentTrack != null)
			{
				if (studentTrack.CurrentTime + seconds >= studentTrack.Duration)
				{
					studentTrack.Pause();
				}
				else
				{
					Sync();
					if (!studentTrack.Playing && teacherTrack.Playing)
					{
						studentTrack.Play();
					}
				}

			}

			OnProgress(teacherTrack.CurrentTime); //Initiating redraw of UI
		}


		/// <summary>
		/// Prepares the teacher track, i.e. makes it ready for playback.
		/// </summary>
		/// <param name="recording">Recording.</param>
		public void PrepareTeacherTrack(Recording recording)
		{
			NSUrl url = NSUrl.FromString(FileUtil.getAbsolutePath(recording));
			teacherTrack = AVAudioPlayer.FromUrl(url);
			teacherTrack.Volume = teacherTrackVolume;

			teacherTrack.PrepareToPlay();
			teacherTrack.FinishedPlaying += (sender, e) =>
			{
				//we subscribe to the native stop event of the player and call our own stop method
				Stop();
				OnProgress(teacherTrack.Duration);
			};
			State = PlayerState.STOPPED;
			OnProgress(teacherTrack.CurrentTime);
		}

		/// <summary>
		/// Prepares the student track, i.e. makes it ready for playback.
		/// Can be called with a null parameter to reset the student player
		/// </summary>
		/// <param name="recording">Recording.</param>
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
				studentTrack.Volume = studentTrackVolume;
				studentTrack.PrepareToPlay();
			}

		}

		/// <summary>
		/// Requesting permissions and activating the audio session.
		/// </summary>
		public void ActivateAudioSession()
		{
			var status = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Audio);
			AVAudioSession session = AVAudioSession.SharedInstance();
			session.SetCategory(AVAudioSessionCategory.PlayAndRecord);

			adjustAudioOutputPort(session);

			session.SetActive(true);
			if (status == AVAuthorizationStatus.NotDetermined)
			{
				session.RequestRecordPermission((granted) => {
					//Nothing to do here.
				});
			}

		}

		/// <summary>
		/// Depending on whether a headset/headphones are plugged in, the audio output is 
		/// set correctly.
		/// </summary>
		/// <param name="session">Session.</param>
		private void adjustAudioOutputPort(AVAudioSession session)
		{
			NSError err;
			AVAudioSessionPortOverride outputPort = isHeadphonePluggedIn() ? AVAudioSessionPortOverride.None : AVAudioSessionPortOverride.Speaker;
			session.OverrideOutputAudioPort(outputPort, out err);
		}

		/// <summary>
		/// Checks whether a headphone is plugged in the device.
		/// </summary>
		/// <returns><c>true</c>, if a headphone is plugged in, <c>false</c> otherwise.</returns>
		private bool isHeadphonePluggedIn()
		{
			AVAudioSessionPortDescription[] availableOutputs = AVAudioSession.SharedInstance().CurrentRoute.Outputs;
			foreach (AVAudioSessionPortDescription desc in availableOutputs)
			{
				if (desc.PortType == AVAudioSession.PortHeadphones)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// This method is called via the elapsed timer to create a DrawUpdate for the view
		/// with the current playback progess in milliseconds
		/// </summary>
		private void OnProgress(double currentProgress)
		{
			var milliseconds = (int)(currentProgress * 1000);
			Update?.Invoke(milliseconds);

		}


		/// <summary>
		/// This method notifies all subscribers of the StateChange event.
		/// </summary>
		private void OnStateChange()
		{
			StateChange?.Invoke(state);
		}


		/// <summary>
		/// Synchronizing playback time of student and teachertrack.
		/// </summary>
		private void Sync()
		{
                studentTrack.CurrentTime = teacherTrack.CurrentTime;   
		}



	}
}
