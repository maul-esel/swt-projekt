using System;
using Lingvo.Common;

namespace MobileApp
{
	/// <summary>
	/// Controller for handling teacher memos.
	/// </summary>
	public class TeacherMemoController
	{
		private IPlayer player;
		private IRecorder recorder;

		private LocalCollection localCollection;

		public TeacherMemoController()
		{
			// to do: initialize player & recorder
		}

		/// <summary>
		/// Starts a new teacher memo.
		/// </summary>
		public void StartTeacherMemo()
		{
			recorder.Start();
		}

		/// <summary>
		/// Ends the teacher memo started before with the start method.
		/// </summary>
		/// <returns>The teacher memo.</returns>
		public TeacherMemo EndTeacherMemo()
		{
			return (TeacherMemo) recorder.Stop();
		}

		/// <summary>
		/// Restarts the teacher memo.
		/// </summary>
		public void RestartTeacherMemo()
		{
			recorder.Start();
		}

		/// <summary>
		/// Saves the teacher memo to the local collection.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="recordFile">Record file.</param>
		public void SaveTeacherMemo(String name, TeacherMemo recordFile)
		{
			recordFile.Name = name;
			localCollection.AddTeacherMemo(recordFile);

		}

		/// <summary>
		/// Plaies the teacher memo.
		/// </summary>
		/// <param name="memo">Memo.</param>
		public void PlayTeacherMemo(TeacherMemo memo)
		{
			player.Play(memo);
		}
	}
}
