using Lingvo.Common.Adapters;
using Lingvo.Common.Entities;
using System;

namespace MobileApp.Controllers
{
    /// <summary>
    /// Controller for working on pages
    /// </summary>
    public class StudentPageController
	{
		private IPlayer player;
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
			}
		}

		public StudentPageController()
		{
		}

		/// <summary>
		/// Plays the page.
		/// </summary>
		public void PlayPage()
		{
			player.Play(selectedPage.TeacherTrack);
		}

		/// <summary>
		/// Starts the student recording.
		/// </summary>
		public void StartStudentRecording()
		{
			PlayPage();
			recorder.Start();
		}

		/// <summary>
		/// Pauses the recording and/or playing.
		/// </summary>
		public void Pause()
		{
			recorder.Pause();
			player.Pause();
		}

		/// <summary>
		/// Continues the recording and/or playing.
		/// </summary>
		public void Continue()
		{
			recorder.Continue();
			player.Continue();
		}

		/// <summary>
		/// Stops the recording and/or playing, saves the recorded file as student track
		/// </summary>
		public void Stop()
		{
			player.Stop();
			Recording recording = recorder.Stop();
			selectedPage.StudentTrack = recording;
		}

		/// <summary>
		/// Seeks the recording and/or playing to the given time.
		/// </summary>
		public void SeekTo(TimeSpan seek)
		{
			recorder.SeekTo(seek);
			player.SeekTo(seek);
		}
	}
}
