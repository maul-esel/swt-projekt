using System;
using Lingvo.Common.Entities;
using Lingvo.Common.Adapters;

namespace Lingvo.Backend
{
	 /// <summary>
	 /// This class is used for controlling the editor system
	 /// </summary>/

	public class EditorController
	{
		private IRecorder recorder;
		private IPlayer player;

		private Page CurrentPage
		{
			get;
			set;
		}

		private Workbook CurrentWorkbook
		{
			get;
			set;
		}

		public EditorController()
		{
		}

		/// <summary>
		/// Creates a new page.
		/// </summary>
		/// <param name="workbook">Workbook.</param>
		/// <param name="pageNumber">Page number.</param>
		public void CreatePage(Workbook workbook, int pageNumber)
		{
			CurrentPage = new Page();
		}

		/// <summary>
		/// Begins the recording.
		/// </summary>
		public void BeginRecording()
		{
			recorder.Start();
		}

		/// <summary>
		/// Ends the recording.
		/// </summary>
		/// <returns>The recording since method call StartRecording.</returns>
		public Recording EndRecording()
		{
			return recorder.Stop();
		}

		/// <summary>
		/// Imports an exisiting mp3 file for use as recording.
		/// </summary>
		/// <returns>The recording.</returns>
		/// <param name="filename">Filename.</param>
		public Recording ImportRecording(String filename)
		{
			// to do: import mp3 from filesystem
			return null;
		}

		/// <summary>
		/// Saves the page object in the database.
		/// </summary>
		public void savePage()
		{
			// to do: save page in database
		}
	}
}
