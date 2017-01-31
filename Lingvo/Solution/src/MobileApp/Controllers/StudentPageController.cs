using Lingvo.Common.Adapters;
using Lingvo.Common.Entities;
using Lingvo.Common.Enums;
using Lingvo.Common.Services;
using System;
using Xamarin.Forms;
using System.IO;

namespace Lingvo.MobileApp.Controllers
{
    /// <summary>
    /// Controller for working on pages
    /// </summary>
    public class StudentPageController
	{
		private static StudentPageController instance;

		private IPlayer audioPlayer;
		private IRecorder recorder;

		private IPage selectedPage;

		/// <summary>
		/// Gets or sets the selected page.
		/// </summary>
		/// <value>The selected page.</value>
		public IPage SelectedPage 
		{ 
			get
			{
				return selectedPage;
			}

			set
			{
				selectedPage = value;

				audioPlayer.StateChange += (obj) => StopRecorderIfNecessary();
				audioPlayer.PrepareTeacherTrack(selectedPage.TeacherTrack);


				if (selectedPage.StudentTrack != null)
				{
					audioPlayer.PrepareStudentTrack(selectedPage.StudentTrack);
				}
			}

		}


		/// <summary>
		/// This method delegates all subscribers to the udpate event of the audioplayer
		/// It is intented that the audioView subscribes to this event in order to get the current playback progress.
		/// </summary>
		public event Action<int> Update
		{
			add
			{
				audioPlayer.Update += value;
			}

			remove
			{
				audioPlayer.Update -= value;
			}
		}

		public event Action<PlayerState> StateChange
		{
			add 
			{
				audioPlayer.StateChange += value;
			}
			remove 
			{
				audioPlayer.StateChange -= value;
			}
		}


		/// <summary>
		/// Gets or sets a value indicating whether the 
		/// <see cref="T:Lingvo.MobileApp.Controllers.StudentPageController"/>'s studentTrack is muted.
		/// </summary>
		/// <value><c>true</c> if is muted; otherwise, <c>false</c>.</value>
		public bool IsMuted
		{
			get
			{
				return audioPlayer.IsStudentTrackMuted;
			}

			set
			{
				audioPlayer.IsStudentTrackMuted = value;
			}
				
				
		}

		public PlayerState CurrentPlayerState => audioPlayer.State;
		public RecorderState CurrentRecorderState => recorder.State;

		/// <summary>
		/// Gets the instance of StudentPageController (Singleton Pattern)
		/// </summary>
		/// <returns>The instance.</returns>
		public static StudentPageController Instance => instance ?? (instance = new StudentPageController());

		private StudentPageController()
		{
			audioPlayer = DependencyService.Get<IPlayer>();
			recorder = DependencyService.Get<IRecorder>();
		}

		/// <summary>
		/// Plays the page.
		/// </summary>
		public void PlayPage()
		{
			audioPlayer.Play();
		}

		/// <summary>
		/// Starts the student recording.
		/// </summary>
		public void StartStudentRecording()
		{
			if (recorder.State != RecorderState.PREPARED)
			{
				recorder.PrepareToRecord();
			}
			audioPlayer.IsStudentTrackMuted = true;
			recorder.Start();
			PlayPage();

		}

		/// <summary>
		/// Pauses the recording and/or playing.
		/// </summary>
		public void Pause()
		{
			audioPlayer.Pause();
			recorder.Pause();

		}

		/// <summary>
		/// Continues the recording and/or playing.
		/// </summary>
		public void Continue()
		{
			if (recorder.State == RecorderState.PAUSED)
			{
				recorder.Continue();
			}
			audioPlayer.Play();
		}

		/// <summary>
		/// Stops the recording and/or playing, saves the recorded file as student track
		/// </summary>
		public void Stop()
		{
			audioPlayer.Stop();
		}

		/// <summary>
		/// Seeks the recording and/or playing to the given time.
		/// </summary>
		public void SeekTo(int seconds)
		{
			if (recorder.State != RecorderState.RECORDING)
			{
                audioPlayer.SeekTo(seconds);
            }
		}

		private void StopRecorderIfNecessary()
		{
			if (audioPlayer.State == PlayerState.STOPPED && recorder.State == RecorderState.RECORDING)
			{
				//This means an recording-session has come to an end. Thus we are deleting the old studentTrack-Recording
				//if necessary and adding the new one to the selectedPage

				//Getting new recording
                Recording recording = recorder.Stop();

				//deleting the old recording
				if (selectedPage.StudentTrack != null)
				{
					File.Delete(FileUtil.getAbsolutePath(selectedPage.StudentTrack)); 	
				}

				SelectedPage.StudentTrack = recording;
				var db = App.Database;
				db.Save(recording);
				db.Save((Lingvo.Common.Entities.Page) SelectedPage);

				//Setting the new recording
				SelectedPage.StudentTrack = recording;

				//Reset the page
                SelectedPage = selectedPage;
            }
		}

	}
}
