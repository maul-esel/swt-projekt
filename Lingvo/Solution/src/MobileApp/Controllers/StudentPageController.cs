using Lingvo.Common.Adapters;
using Lingvo.Common.Entities;
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

				/*var documentsDirPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				var filePath = Path.Combine(documentsDirPath, "Content/home.mp3");
				var recording = new Recording(id: 99, length: TimeSpan.FromSeconds(231), localPath: filePath, creationTime: new DateTime());
				audioPlayer.PrepareTeacherTrack(recording);*/
				audioPlayer.PrepareTeacherTrack(selectedPage.TeacherTrack);

				if (selectedPage.StudentTrack != null)
				{
					audioPlayer.PrepareStudentTrack(selectedPage.TeacherTrack);
				}
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


		/// <summary>
		/// Gets the instance of StudentPageController (Singleton Pattern)
		/// </summary>
		/// <returns>The instance.</returns>
		public static StudentPageController Instance => instance ?? (instance = new StudentPageController());

		private StudentPageController()
		{
			audioPlayer = DependencyService.Get<IPlayer>();
			//recorder = DependencyService.Get<IRecorder>();
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
			PlayPage();
			recorder?.Start();
		}

		/// <summary>
		/// Pauses the recording and/or playing.
		/// </summary>
		public void Pause()
		{
			recorder?.Pause();
			audioPlayer.Pause();
		}

		/// <summary>
		/// Continues the recording and/or playing.
		/// </summary>
		public void Continue()
		{
			recorder.Continue();
			audioPlayer.Continue();
		}

		/// <summary>
		/// Stops the recording and/or playing, saves the recorded file as student track
		/// </summary>
		public void Stop()
		{
			audioPlayer.Stop();
			Recording recording = recorder?.Stop();
			SelectedPage.StudentTrack = recording;
		}

		/// <summary>
		/// Seeks the recording and/or playing to the given time.
		/// </summary>
		public void SeekTo(TimeSpan timeCode)
		{
			recorder.SeekTo(timeCode);
			audioPlayer.SeekTo(timeCode);
		}
	}
}
