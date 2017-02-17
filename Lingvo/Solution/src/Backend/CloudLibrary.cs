using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Lingvo.Backend
{
	using Common.Entities;

	public class CloudLibrary
	{	
		private readonly IStorage _storage;
		private readonly DatabaseService _dbservice;

		public CloudLibrary(IStorage storage, DatabaseService dbservice)
		{
			_storage = storage;
			_dbservice = dbservice;
		}

		public async Task Save(Recording recording)
		{
			_dbservice.Save(recording);
		}

		public async Task Save(Page page)
		{
			_dbservice.Save(page);
		}

		public async Task Save(Workbook workbook)
		{
			_dbservice.Save(workbook);
		}

		public async Task Delete(Recording recording)
		{
			recording = await _dbservice.Recordings.FindAsync(recording.Id);
			if (recording == null)
				return;

			await _storage.DeleteAsync(recording.LocalPath);
			_dbservice.Delete(recording);
		}

		public async Task Delete(Page page)
		{
			page = await _dbservice.Pages.FindAsync(page.Id);
			if (page == null)
				return;

			await Delete(page.TeacherTrack);
			_dbservice.Delete(page);
		}

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

		public Recording FindRecording(int id)
		{
			return _dbservice.Recordings.Find(id);
		}

		public Page FindPageWithRecording(int id)
		{
			return _dbservice.FindPageWithRecording(id);
		}

		public Task<Page> FindPageByNumberAsync(int workbookId, int pageNumber)
		{
			return _dbservice.Pages.FirstOrDefaultAsync(p => p.workbookId == workbookId && p.Number == pageNumber);
		}

		public Workbook FindWorkbookWithReferences(int id)
		{
			return _dbservice.FindWorkbookWithReferences(id);
		}

		public List<Workbook> FindWorkbooksWithReferences()
		{
			return _dbservice.GetWorkbooksWithReferences();
		}

		public async Task<string> GetAccessUrlAsync(Recording recording)
		{
			return await _storage.GetAccessUrlAsync(recording.LocalPath);
		}

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
