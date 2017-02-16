using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

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
			var r = _dbservice.Recordings.Find(recording.Id);

			if (r != null)
			{
				await DeleteRecordingFile(r.LocalPath);
				_dbservice.Recordings.Remove(r);
				_dbservice.SaveChanges();
			}

			_dbservice.Delete(recording);
		}

		public async Task Delete(Page page)
		{

			var p = _dbservice.Pages.Find(page.Id);

			if (p == null)
			{
				return;
			}
			var r = _dbservice.Recordings.Find(page.teacherTrackId);

			if ( r != null)
			{
				await DeleteRecordingFile(r.LocalPath);
			}
			_dbservice.Delete(page);
		}

		public async Task Delete(Workbook workbook)
		{
			_dbservice.Delete(workbook);
		}

		private async Task DeleteRecordingFile(string fileName)
		{
			await _storage.DeleteAsync(fileName);
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

		public async Task<string> GetAccessUrlAsync(string identifier)
		{
			var url = await _storage.GetAccessUrlAsync(identifier);

			return url;
		}

		public async Task DeleteAsyncFromStorage(string identifier)
		{
			await _storage.DeleteAsync(identifier);
		}

		public async Task SaveAsyncFromStorage(string identifier, Stream content)
		{
			await _storage.SaveAsync(identifier, content);
		}

		public async Task<Recording> SaveRecordingToStorage(IFormFile file, string fileName, int duration)
		{
			Stream stream = null;
			try
			{
				stream = file.OpenReadStream();
				await _storage.SaveAsync(fileName, stream);
			}
			finally
			{
				stream?.Dispose();
			}

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
