using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using Lingvo.Backend.ViewModels;
using Lingvo.Common.Entities;


namespace Lingvo.Backend.Controllers
{
	#if !DEBUG
		[RequireHttps]
	#endif
	[Authorize]
	public class PageController : Controller
	{
		[Route("pages/edit/{id}")]
		public async Task<IActionResult> EditPage([FromServices] DatabaseService db, [FromServices] IStorage storage, int id)
		{
			ViewData["Title"] = "Seite bearbeiten";

			var page = db.FindPageWithRecording(id);
			var workbook = await db.FindAsync<Workbook>(page.workbookId);
			var recordingUrl = await storage.GetAccessUrlAsync(page.TeacherTrack.LocalPath);

			var model = new PageModel(page, recordingUrl) { Workbook = workbook };
			return View("AddPage", model);
		}

		[Route("pages/delete/{id}")]
		public IActionResult DeletePage([FromServices] DatabaseService db, int id)
		{
			var page = db.Find<Page>(id);
			var workbookID = page.workbookId;
			db.Delete(page);
			return RedirectToAction(nameof(Workbook), new { id = workbookID });
		}

		[HttpPost]
		public async Task<IActionResult> UpdatePage([FromServices] DatabaseService db, [FromServices] IStorage storage, int id, PageModel model)
		{
			var page = db.FindPageWithRecording(id);

			if (model.UploadedFile != null || model.RecordedFile != null)
			{
				var recording = await SaveRecording(db, storage, model);
				await storage.DeleteAsync(page.TeacherTrack.LocalPath);
				db.Delete(page.TeacherTrack);
				page.TeacherTrack = recording;
			}

			page.Description = model.Description;
			page.Number = model.PageNumber;
			db.Save(page);

			return RedirectToAction(nameof(Workbook), new { id = model.WorkbookID });
		}

		[HttpPost]
		public async Task<IActionResult> CreatePage([FromServices] DatabaseService db, [FromServices] IStorage storage, PageModel model)
		{
			var recording = await SaveRecording(db, storage, model);
			db.Save(new Page()
			{
				Number = model.PageNumber,
				Description = model.Description,
				workbookId = model.WorkbookID,
				teacherTrackId = recording.Id
			});
			return RedirectToAction(nameof(Workbook), new { id = model.WorkbookID });
		}

		private async Task<Recording> SaveRecording(DatabaseService db, IStorage storage, PageModel model)
		{
			var file = model.RecordedFile ?? model.UploadedFile;
			if (file == null)
				throw new InvalidDataException("no file uploaded");

			string prefix = (model.RecordedFile == null) ? "uploaded_" : "recorded_";
			string fileName = prefix + Guid.NewGuid().ToString();

			Stream stream = null;
			try
			{
				stream = file.OpenReadStream();
				await storage.SaveAsync(fileName, stream);
			}
			finally
			{
				stream?.Dispose();
			}

			var recording = new Recording()
			{
				Duration = model.Duration,
				LocalPath = fileName
			};
			db.Save(recording);

			return recording;
		}
	
	}
}
