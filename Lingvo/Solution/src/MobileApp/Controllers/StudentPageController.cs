using Lingvo.Common.Adapters;
using Lingvo.Common.Entities;
using System;

namespace Lingvo.MobileApp.Controllers
{
    /// <summary>
    /// Controller for working on pages
    /// </summary>
    public class StudentPageController
	{
		private IPlayer teacherTrackPlayer;
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
			teacherTrackPlayer.Play(selectedPage.TeacherTrack);
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
			teacherTrackPlayer.Pause();
		}

		/// <summary>
		/// Continues the recording and/or playing.
		/// </summary>
		public void Continue()
		{
			recorder.Continue();
			teacherTrackPlayer.Continue();
		}

		/// <summary>
		/// Stops the recording and/or playing, saves the recorded file as student track
		/// </summary>
		public void Stop()
		{
			teacherTrackPlayer.Stop();
			Recording recording = recorder.Stop();
			selectedPage.StudentTrack = recording;
		}

		/// <summary>
		/// Seeks the recording and/or playing to the given time.
		/// </summary>
		public void SeekTo(TimeSpan seek)
		{
			recorder.SeekTo(seek);
			teacherTrackPlayer.SeekTo(seek);
		}
	}
}
