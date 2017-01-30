using Lingvo.Common.Adapters;
using Lingvo.MobileApp.Entities;
using System;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Controllers
{
    /// <summary>
    /// Controller for handling teacher memos.
    /// </summary>
    public class TeacherMemoController
	{
		private static TeacherMemoController instance;
		private IPlayer player;
		private IRecorder recorder;

		/// <summary>
		/// Gets the instance of the teacherMemoController (Singleton Pattern)
		/// </summary>
		/// <value>The instance.</value>
		public static TeacherMemoController Instance => instance ?? (instance = new TeacherMemoController());

		private TeacherMemoController()
		{
			player = DependencyService.Get<IPlayer>();
			//TODO: recorder = DependencyController.Get<IRecorder>();

		}

		/// <summary>
		/// Starts a new teacher memo.
		/// </summary>
		public void StartTeacherMemo()
		{
			//TODO: recorder.Start();
		}

		/// <summary>
		/// Ends the teacher memo started before with the start method.
		/// </summary>
		/// <returns>The teacher memo.</returns>
		public TeacherMemo EndTeacherMemo()
		{
			return new TeacherMemo() { Recording = recorder.Stop() };
		}

		/// <summary>
		/// Restarts the teacher memo.
		/// </summary>
		public void RestartTeacherMemo()
		{
			//TODO: recorder.Start();
		}

		/// <summary>
		/// Saves the teacher memo to the local collection.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="recordFile">Record file.</param>
		public void SaveTeacherMemo(String name, TeacherMemo recordFile)
		{
			recordFile.Name = name;
			LocalCollection.Instance.AddTeacherMemo(recordFile);

		}

		/// <summary>
		/// Plaies the teacher memo.
		/// </summary>
		/// <param name="memo">Memo.</param>
		public void PlayTeacherMemo(TeacherMemo memo)
		{
			player.PrepareTeacherTrack(memo.Recording);
			player.Play();
		}
	}
}
