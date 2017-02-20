using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Lingvo.Backend.Services
{
	using Common.Entities;

	/// <summary>
	/// Encapsulates all data storage, both database (<see cref="DatabaseService"/>)
	/// and files (<see cref="IStorage"/>).
	/// </summary>
	public class CloudLibrary
	{	
		private readonly IStorage _storage;
		private readonly DatabaseService _dbservice;

		public CloudLibrary(IStorage storage, DatabaseService dbservice)
		{
			_storage = storage;
			_dbservice = dbservice;
		}

		/// <summary>
		/// Saves changes in the given <paramref name="recording"/> to the database.
		/// </summary>
		/// <returns>A <see cref="Task"/> that completes once the save process is complete.</returns>
		public async Task Save(Recording recording)
		{
			_dbservice.Save(recording);
		}

		/// <summary>
		/// Saves changes in the given <paramref name="page"/> to the database.
		/// </summary>
		/// <returns>A <see cref="Task"/> that completes once the save process is complete.</returns>
		public async Task Save(Page page)
		{
			_dbservice.Save(page);
		}

		/// <summary>
		/// Saves changes in the given <paramref name="workbook"/> to the database.
		/// </summary>
		/// <param name="workbook"></param>
		/// <returns>A <see cref="Task"/> that completes once the save process is complete.</returns>
		public async Task Save(Workbook workbook)
		{
			_dbservice.Save(workbook);
		}

		/// <summary>
		/// Deletes the given <paramref name="recording"/> from the database.
		/// Also deltes the associated file from the file storage.
		/// </summary>
		/// <returns>A <see cref="Task"/> that completes once the delete process is complete.</returns>
		public async Task Delete(Recording recording)
		{
			recording = await _dbservice.Recordings.FindAsync(recording.Id);
			if (recording == null)
				return;

			await _storage.DeleteAsync(recording.LocalPath);
			_dbservice.Delete(recording);
		}

		/// <summary>
		/// Deletes the given <paramref name="page"/> from the database.
		/// Also deletes its associated <see cref="Page.TeacherTrack"/> including
		/// the binary audio file.
		/// </summary>
		/// <returns>A <see cref="Task"/> that completes once the delete process is complete.</returns>
		public async Task Delete(Page page)
		{
			page = await _dbservice.Pages.FindAsync(page.Id);
			if (page == null)
				return;

			await Delete(page.TeacherTrack);
			_dbservice.Delete(page);
		}

		/// <summary>
		/// Deletes the given <paramref name="workbook"/> including all its
		/// pages and their <see cref="Page.TeacherTrack"/>s.
		/// </summary>
		/// <returns>A <see cref="Task"/> that completes once the delete process is complete.</returns>
		public async Task Delete(Workbook workbook)
		{
			workbook = _dbservice.FindWorkbookWithReferences(workbook.Id);
			if (workbook == null)
				return;

			// delete pages explicitly here:
			// also deletes their recordings from storage
			foreach (var page in workbook.Pages.ToList())
				await Delete((Page)page);

			_dbservice.Delete(workbook);
		}

		/// <summary>
		/// Finds the <see cref="Recording"/> with the given <see cref="Recording.Id"/>.
		/// </summary>
		public Recording FindRecording(int id)
		{
			return _dbservice.Recordings.Find(id);
		}

		/// <summary>
		/// Finds the <see cref="Page"/> with the given <see cref="Page.Id"/>.
		/// Also loads the associated <see cref="Page.TeacherTrack"/>.
		/// </summary>
		public Page FindPageWithRecording(int id)
		{
			return _dbservice.FindPageWithRecording(id);
		}

		/// <summary>
		/// Finds the <see cref="Page"/> with the given <paramref name="pageNumber"/>
		/// and belonging to the workbook with the given <paramref name="workbookId"/>.
		/// </summary>
		/// <returns>A <see cref="Task"/> that eventually completes and returns the page.</returns>
		public Task<Page> FindPageByNumberAsync(int workbookId, int pageNumber)
		{
			return _dbservice.Pages.FirstOrDefaultAsync(p => p.workbookId == workbookId && p.Number == pageNumber);
		}

		/// <summary>
		/// Finds the <see cref="Workbook"/> with the given <paramref name="title"/>.
		/// </summary>
		/// <returns>A <see cref="Task"/> that eventually completes and returns the workbook.</returns>
		public Task<Workbook> FindWorkbookByTitleAsync(string title)
		{
			return _dbservice.Workbooks.FirstOrDefaultAsync(w => w.Title == title);
		}

		/// <summary>
		/// Finds the <see cref="Workbook"/> with the given <paramref name="id"/>.
		/// Also loads all associated pages and their recordings.
		/// </summary>
		public Workbook FindWorkbookWithReferences(int id)
		{
			return _dbservice.FindWorkbookWithReferences(id);
		}

		/// <summary>
		/// Loads all workbooks, their pages and the associated recordings from the
		/// database.
		/// </summary>
		/// <returns></returns>
		public List<Workbook> FindWorkbooksWithReferences()
		{
			return _dbservice.GetWorkbooksWithReferences();
		}

		/// <summary>
		/// Retrieves the download URL for the given <paramref name="recording"/>.
		/// </summary>
		/// <returns>A URL pointing to the underlying MP3 file.</returns>
		public async Task<string> GetAccessUrlAsync(Recording recording)
		{
			return await _storage.GetAccessUrlAsync(recording.LocalPath);
		}

		/// <summary>
		/// Creates a new <see cref="Recording"/> and saves it.
		/// </summary>
		/// <param name="content">A stream containing the <see cref="Recording"/>'s MP3 file.</param>
		/// <param name="fileName">The file name, or identifier, to be used for the recording.</param>
		/// <param name="duration">The length of the MP3 file, in milliseconds.</param>
		/// <returns>A <see cref="Task"/> that eventually completes and returns the new <see cref="Recording"/>.</returns>
		public async Task<Recording> CreateRecording(Stream content, string fileName, int duration)
		{
			await _storage.SaveAsync(fileName, content);

			var recording = new Recording()
			{
				Duration = duration,
				LocalPath = fileName
			};
			_dbservice.Save(recording);

			return recording;
		}
	}
}
